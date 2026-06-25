using Microsoft.EntityFrameworkCore;
using SantoExpedito.Data;
using SantoExpedito.Models;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Services;

public class ClienteService : IClienteService
{
    private readonly AppDbContext _context;

    public ClienteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cliente>> GetAllAsync()
    {
        return await _context.Clientes.OrderBy(c => c.Nome).ToListAsync();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _context.Clientes.FindAsync(id);
    }

    public async Task<Cliente> CreateAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        var existing = await _context.Clientes.FindAsync(cliente.Id);
        if (existing == null) return;

        existing.Nome = cliente.Nome;
        existing.Telefone = cliente.Telefone;
        existing.Endereco = cliente.Endereco;
        existing.PetNome = cliente.PetNome;
        existing.PetRaca = cliente.PetRaca;
        existing.Porte = cliente.Porte;
        existing.Plano = cliente.Plano;
        existing.BanhosRestantes = cliente.BanhosRestantes;
        existing.Taxidog = cliente.Taxidog;
        existing.Tosa = cliente.Tosa;
        existing.Valor = cliente.Valor;
        existing.DiasBanho = cliente.DiasBanho;
        existing.DatasBanho = cliente.DatasBanho;
        existing.PetRacao = cliente.PetRacao;
        existing.PetVermifugacao = cliente.PetVermifugacao;
        existing.PetAntipulgas = cliente.PetAntipulgas;
        existing.PetVacinacao = cliente.PetVacinacao;
        existing.Cpf = cliente.Cpf;
        existing.Email = cliente.Email;
        existing.PetIdade = cliente.PetIdade;

        await UpdatePetAsync(existing);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente != null)
        {
            var pets = await _context.Pets.Where(p => p.Nome == cliente.PetNome && p.Tutor == cliente.Nome).ToListAsync();
            _context.Pets.RemoveRange(pets);
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdatePetAsync(Cliente cliente)
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
        }
    }
}
