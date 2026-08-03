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
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var clientes = await _context.Clientes.ToListAsync();
        var vencimentos = await _context.Vencimentos.ToListAsync();
        sw.Stop();

        var hoje = DateTime.Now;
        var hojeStr = $"{hoje.Day:D2}/{hoje.Month:D2}/{hoje.Year}";
        var hojeInicio = new DateTime(hoje.Year, hoje.Month, hoje.Day);

        int pagos = 0, pendentes = 0, atrasados = 0, vencemHoje = 0, vencemProx7 = 0;
        foreach (var v in vencimentos)
        {
            if (string.Equals(v.Status, "Pago", StringComparison.OrdinalIgnoreCase))
            {
                pagos++;
                continue;
            }

            var partes = (v.VencimentoData ?? "").Split('/');
            if (partes.Length == 3 && DateTime.TryParse($"{partes[2]}-{partes[1]}-{partes[0]}", out var dt))
            {
                var diff = (dt - hojeInicio).Days;
                if (diff < 0)
                    atrasados++;
                else if (diff == 0)
                    vencemHoje++;
                else if (diff <= 7)
                    vencemProx7++;

                if (string.Equals(v.Status, "Pendente", StringComparison.OrdinalIgnoreCase))
                    pendentes++;
            }
            else
            {
                if (string.Equals(v.Status, "Pendente", StringComparison.OrdinalIgnoreCase))
                    pendentes++;
            }
        }

        double dbSize = 0;
        try
        {
            var conn = _context.Database.GetConnectionString();
            var dataSource = conn?.Split(';')
                .Select(p => p.Trim())
                .FirstOrDefault(p => p.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
                ?.Substring("Data Source=".Length);
            if (!string.IsNullOrEmpty(dataSource) && System.IO.File.Exists(dataSource))
                dbSize = new System.IO.FileInfo(dataSource).Length / (1024.0 * 1024.0);
        }
        catch { }

        return new DashboardDto
        {
            TotalClientes = clientes.Count,
            TotalPets = await _context.Pets.CountAsync(),
            PlanosMensais = clientes.Count(c => c.Plano == "Mensal"),
            PlanosQuinzenais = clientes.Count(c => c.Plano == "Quinzenal"),
            PagamentosPagos = pagos,
            PagamentosPendentes = pendentes,
            PagamentosAtrasados = atrasados,
            PagamentosVencemHoje = vencemHoje,
            PagamentosVencemProx7Dias = vencemProx7,
            AgendamentosHoje = await _context.Agendamentos
                .Where(a => a.Data == hojeStr && a.Status != "Concluído" && a.Status != "Removido")
                .ToListAsync(),
            Vencimentos = vencimentos.OrderBy(v => v.VencimentoData).Take(10).ToList(),
            DbSizeMb = Math.Round(dbSize, 2),
            DbResponseTimeMs = sw.ElapsedMilliseconds
        };
    }
}
