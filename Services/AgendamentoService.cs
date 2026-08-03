using Microsoft.EntityFrameworkCore;
using SantoExpedito.Data;
using SantoExpedito.Models;
using SantoExpedito.Services.Interfaces;
using SantoExpedito.Services.RegrasAgendamento;

namespace SantoExpedito.Services;

public class AgendamentoService : IAgendamentoService
{
    private readonly AppDbContext _context;
    private readonly ServicoRegrasAgendamento _regras;

    public AgendamentoService(AppDbContext context, ServicoRegrasAgendamento regras)
    {
        _context = context;
        _regras = regras;
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

                var dataOriginal = new DateTime(mesAlvo.Year, mesAlvo.Month, dia);
                var dataAjustada = _regras.Aplicar(dataOriginal);
                var dataStr = $"{dataAjustada.Day:D2}/{dataAjustada.Month:D2}/{dataAjustada.Year}";

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

    public List<(int DiaFixo, DateTime DataReal, bool Ajustado)> ObterDatasProximosBanhos(string plano, string datasBanho)
    {
        var resultado = new List<(int DiaFixo, DateTime DataReal, bool Ajustado)>();
        if (string.IsNullOrWhiteSpace(datasBanho)) return resultado;

        var dias = datasBanho
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(d => int.TryParse(d.Trim(), out var dia) ? dia : (int?)null)
            .Where(d => d.HasValue && d.Value >= 1 && d.Value <= 31)
            .Select(d => d!.Value)
            .ToList();

        if (dias.Count == 0) return resultado;

        var hoje = DateTime.Now;

        for (int m = 0; m < 2; m++)
        {
            var mesAlvo = hoje.AddMonths(m);
            var diasNoMes = DateTime.DaysInMonth(mesAlvo.Year, mesAlvo.Month);

            foreach (var dia in dias)
            {
                if (dia > diasNoMes) continue;

                var dataOriginal = new DateTime(mesAlvo.Year, mesAlvo.Month, dia);
                var dataAjustada = _regras.Aplicar(dataOriginal);
                var ajustada = dataAjustada != dataOriginal;

                resultado.Add((dia, dataAjustada, ajustada));
            }
        }

        return resultado;
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

    public async Task<int> RemoverOrfaosAsync()
    {
        var agendamentos = await _context.Agendamentos.ToListAsync();
        var petNomes = await _context.Pets.Select(p => new { p.Nome, p.Tutor }).ToListAsync();
        var tutorNomes = await _context.Clientes.Select(c => new { c.Nome, c.PetNome }).ToListAsync();

        var paraRemover = agendamentos.Where(a =>
            !petNomes.Any(p => p.Nome == a.Pet && p.Tutor == a.Tutor) &&
            !tutorNomes.Any(c => c.Nome == a.Tutor && c.PetNome == a.Pet)
        ).ToList();

        if (paraRemover.Count > 0)
        {
            _context.Agendamentos.RemoveRange(paraRemover);
            await _context.SaveChangesAsync();
        }
        return paraRemover.Count;
    }
}
