using Microsoft.EntityFrameworkCore;
using SantoExpedito.Data;
using SantoExpedito.Models;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Services;

public class PetService : IPetService
{
    private readonly AppDbContext _context;

    public PetService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Pet>> GetAllAsync(string? searchTerm = null)
    {
        var query = _context.Pets.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(p =>
                p.Nome.ToLower().Contains(term) ||
                (p.Tutor != null && p.Tutor.ToLower().Contains(term)) ||
                (p.Raca != null && p.Raca.ToLower().Contains(term)) ||
                (p.Porte != null && p.Porte.ToLower().Contains(term))
            );
        }

        return await query.OrderBy(p => p.Nome).ToListAsync();
    }

    public async Task<Pet?> GetByIdAsync(int id)
    {
        return await _context.Pets.FindAsync(id);
    }

    public async Task CreateAsync(Pet pet)
    {
        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateByClienteAsync(Cliente cliente)
    {
        var pet = await _context.Pets.FirstOrDefaultAsync(p =>
            p.Nome == cliente.PetNome && p.Tutor == cliente.Nome);
        if (pet != null)
        {
            pet.Raca = cliente.PetRaca;
            pet.Porte = cliente.Porte;
            pet.Racao = cliente.PetRacao;
            pet.Vermifugacao = cliente.PetVermifugacao;
            pet.Antipulgas = cliente.PetAntipulgas;
            pet.Vacinacao = cliente.PetVacinacao;
            pet.Idade = cliente.PetIdade;
            await _context.SaveChangesAsync();
        }
    }
}
