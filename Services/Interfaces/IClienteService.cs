using SantoExpedito.Models;

namespace SantoExpedito.Services.Interfaces;

public interface IClienteService
{
    Task<List<Cliente>> GetAllAsync(string? searchTerm = null);
    Task<Cliente?> GetByIdAsync(int id);
    Task<Cliente?> GetByNomeAsync(string nome);
    Task<Cliente> CreateAsync(Cliente cliente);
    Task<bool> ExistsAsync(int id);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(int id);
    Task UpdatePetAsync(Cliente cliente);
    Task<string> GerarMockDataAsync();
    Task<int> LimparMockDataAsync();
}
