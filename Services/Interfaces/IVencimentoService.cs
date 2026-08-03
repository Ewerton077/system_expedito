using SantoExpedito.DTOs;

namespace SantoExpedito.Services.Interfaces;

public interface IVencimentoService
{
    Task<VencimentoListDto> GetVencimentosAsync();
    Task<FinanceiroDto> GetFinanceiroAsync();
    Task<int> RemoverOrfaosAsync();
    Task AlterarStatusAsync(int id, string status);
    Task RenovarPlanoAsync(int id);
    Task<VencimentoItemDto?> GetByIdAsync(int id);
    Task<VencimentoItemDto?> GetVencimentoClienteAsync(string cliente);
}
