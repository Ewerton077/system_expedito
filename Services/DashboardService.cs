using Microsoft.EntityFrameworkCore;
using SantoExpedito.Data;
using SantoExpedito.DTOs;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> GetDashboardDataAsync()
    {
        var clientes = await _context.Clientes.ToListAsync();
        var vencimentos = await _context.Vencimentos.ToListAsync();

        var hoje = DateTime.Now;
        var hojeStr = $"{hoje.Day:D2}/{hoje.Month:D2}/{hoje.Year}";

        return new DashboardDto
        {
            TotalClientes = clientes.Count,
            TotalPets = await _context.Pets.CountAsync(),
            PlanosMensais = clientes.Count(c => c.Plano == "Mensal"),
            PlanosQuinzenais = clientes.Count(c => c.Plano == "Quinzenal"),
            PagamentosPagos = vencimentos.Count(v => v.Status == "pago"),
            PagamentosPendentes = vencimentos.Count(v => v.Status == "pendente"),
            PagamentosAtrasados = vencimentos.Count(v => v.Status == "pendente"),
            AgendamentosHoje = await _context.Agendamentos
                .Where(a => a.Data == hojeStr && a.Status != "Concluído" && a.Status != "Removido")
                .ToListAsync(),
            Vencimentos = vencimentos.OrderBy(v => v.VencimentoData).Take(10).ToList()
        };
    }
}
