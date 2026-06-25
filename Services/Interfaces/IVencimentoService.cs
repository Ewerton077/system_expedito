using SantoExpedito.DTOs;

namespace SantoExpedito.Services.Interfaces;

public interface IVencimentoService
{
    Task<VencimentoListDto> GetVencimentosAsync();
    Task<FinanceiroDto> GetFinanceiroAsync();
}
