using SantoExpedito.Models;

namespace SantoExpedito.Services.Interfaces;

public interface IPetService
{
    Task<List<Pet>> GetAllAsync(string? searchTerm = null);
    Task<Pet?> GetByIdAsync(int id);
    Task CreateAsync(Pet pet);
    Task UpdateByClienteAsync(Cliente cliente);
    Task DeleteAsync(int id);
}
