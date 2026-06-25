using SantoExpedito.Models;

namespace SantoExpedito.DTOs;

public class DashboardDto
{
    public int TotalClientes { get; set; }
    public int TotalPets { get; set; }
    public int PlanosMensais { get; set; }
    public int PlanosQuinzenais { get; set; }
    public int PagamentosPagos { get; set; }
    public int PagamentosPendentes { get; set; }
    public int PagamentosAtrasados { get; set; }
    public List<Agendamento> AgendamentosHoje { get; set; } = new();
    public List<Vencimento> Vencimentos { get; set; } = new();
}
