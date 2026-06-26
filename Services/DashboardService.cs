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

        double dbSize = 0;
        try
        {
            var conn = _context.Database.GetConnectionString();
            using var sqlConn = new Microsoft.Data.SqlClient.SqlConnection(conn);
            await sqlConn.OpenAsync();
            using var cmd = sqlConn.CreateCommand();
            cmd.CommandText = @"
                SELECT SUM(size * 8 / 1024.0)
                FROM sys.database_files
                WHERE type_desc = 'ROWS'";
            var result = await cmd.ExecuteScalarAsync();
            if (result != null && result != DBNull.Value)
                dbSize = System.Convert.ToDouble(result);
            await sqlConn.CloseAsync();
        }
        catch { }

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
            Vencimentos = vencimentos.OrderBy(v => v.VencimentoData).Take(10).ToList(),
            DbSizeMb = Math.Round(dbSize, 2),
            DbResponseTimeMs = sw.ElapsedMilliseconds
        };
    }
}
