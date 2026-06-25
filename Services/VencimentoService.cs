using Microsoft.EntityFrameworkCore;
using SantoExpedito.Data;
using SantoExpedito.DTOs;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Services;

public class VencimentoService : IVencimentoService
{
    private readonly AppDbContext _context;

    public VencimentoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<VencimentoListDto> GetVencimentosAsync()
    {
        var vencimentos = await _context.Vencimentos.ToListAsync();

        return new VencimentoListDto
        {
            Pagos = vencimentos.Where(v => v.Status == "pago")
                .Select(v => new VencimentoItemDto
                {
                    Id = v.Id, Cliente = v.Cliente, Plano = v.Plano,
                    VencimentoData = v.VencimentoData, Valor = v.Valor, Status = v.Status
                }).ToList(),
            Pendentes = vencimentos.Where(v => v.Status == "pendente")
                .Select(v => new VencimentoItemDto
                {
                    Id = v.Id, Cliente = v.Cliente, Plano = v.Plano,
                    VencimentoData = v.VencimentoData, Valor = v.Valor, Status = v.Status
                }).ToList()
        };
    }

    public async Task<FinanceiroDto> GetFinanceiroAsync()
    {
        var clientes = await _context.Clientes.ToListAsync();

        return new FinanceiroDto
        {
            ClientesMensal = clientes.Count(c => c.Plano == "Mensal"),
            ClientesQuinzenal = clientes.Count(c => c.Plano == "Quinzenal"),
            ReceitaMensal = clientes.Where(c => c.Plano == "Mensal").Sum(c => c.Valor),
            ReceitaQuinzenal = clientes.Where(c => c.Plano == "Quinzenal").Sum(c => c.Valor),
            ReceitaTotal = clientes.Sum(c => c.Valor)
        };
    }
}
