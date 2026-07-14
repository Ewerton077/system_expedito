using Microsoft.EntityFrameworkCore;
using SantoExpedito.Data;
using SantoExpedito.Models;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Services;

public class AgendamentoService : IAgendamentoService
{
    private readonly AppDbContext _context;

    public AgendamentoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Agendamento>> GetAllAsync()
    {
        return await _context.Agendamentos.OrderBy(a => a.Data).ThenBy(a => a.Horario).ToListAsync();
    }

    public async Task UpdateStatusAsync(int id, string status)
    {
        var agendamento = await _context.Agendamentos.FindAsync(id);
        if (agendamento != null)
        {
            agendamento.Status = status;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemarcarAsync(int id, string novaData, string novoHorario)
    {
        var agendamento = await _context.Agendamentos.FindAsync(id);
        if (agendamento != null)
        {
            agendamento.Data = novaData;
            agendamento.Horario = novoHorario;
            agendamento.Status = "Em espera";
            await _context.SaveChangesAsync();
        }
    }

    public async Task GerarAgendamentosRecorrentesAsync(string tutor, string petNome, string plano, string datasBanho, int meses = 12)
    {
        if (string.IsNullOrWhiteSpace(datasBanho)) return;

        var dias = datasBanho
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(d => int.TryParse(d.Trim(), out var dia) ? dia : (int?)null)
            .Where(d => d.HasValue && d.Value >= 1 && d.Value <= 31)
            .Select(d => d!.Value)
            .ToList();

        if (dias.Count == 0) return;

        var hoje = DateTime.Now;
        var lista = new List<Agendamento>();

        for (int m = 0; m < meses; m++)
        {
            var mesAlvo = hoje.AddMonths(m);
            var diasNoMes = DateTime.DaysInMonth(mesAlvo.Year, mesAlvo.Month);

            foreach (var dia in dias)
            {
                if (dia > diasNoMes) continue;

                var dataStr = $"{dia:D2}/{mesAlvo.Month:D2}/{mesAlvo.Year}";

                var existe = await _context.Agendamentos.AnyAsync(a =>
                    a.Data == dataStr && a.Pet == petNome && a.Tutor == tutor);

                if (!existe)
                {
                    lista.Add(new Agendamento
                    {
                        Data = dataStr,
                        Horario = "08:00",
                        Pet = petNome,
                        Tutor = tutor,
                        Plano = plano,
                        Status = "Em espera"
                    });
                }
            }
        }

        if (lista.Count > 0)
        {
            _context.Agendamentos.AddRange(lista);
            await _context.SaveChangesAsync();
        }
    }

    public async Task CriarBanhoUnicoAsync(string tutor, string endereco, string telefone, string petNome, string data, string horario)
    {
        var agendamento = new Agendamento
        {
            Data = data,
            Horario = horario,
            Pet = petNome,
            Tutor = tutor,
            Endereco = endereco,
            Telefone = telefone,
            Plano = "Único",
            Status = "Em espera"
        };
        _context.Agendamentos.Add(agendamento);
        await _context.SaveChangesAsync();
    }
}
