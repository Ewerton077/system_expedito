using SantoExpedito.DTOs;

namespace SantoExpedito.Services.Interfaces;

public interface ICadastroService
{
    Task<string> CreateClienteAsync(CadastroViewModel model);
}
