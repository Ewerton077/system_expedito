using SantoExpedito.Models;

namespace SantoExpedito.Services.Interfaces;

public interface IClienteService
{
    Task<List<Cliente>> GetAllAsync();
    Task<Cliente?> GetByIdAsync(int id);
    Task<Cliente> CreateAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(int id);
    Task UpdatePetAsync(Cliente cliente);
}
