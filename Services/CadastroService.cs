using Microsoft.EntityFrameworkCore;
using SantoExpedito.Data;
using SantoExpedito.DTOs;
using SantoExpedito.Models;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Services;

public class CadastroService : ICadastroService
{
    private readonly AppDbContext _context;

    public CadastroService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> CreateClienteAsync(CadastroViewModel model)
    {
        if (model.Pets == null || model.Pets.Count == 0)
            return "Adicione ao menos um pet";

        var stored = await _context.Clientes.ToListAsync();
        int maxId = stored.Count > 0 ? stored.Max(c => c.Id) : 0;

        var diasBanhoStr = string.Join(",", model.DiasBanho ?? new List<string>());
        var datasBanhoStr = string.Join(",", model.DatasBanho ?? new List<string>());

        foreach (var pet in model.Pets)
        {
            maxId++;
            var cliente = new Cliente
            {
                Nome = model.Responsavel.Nome,
                Telefone = model.Responsavel.Celular,
                Endereco = model.Responsavel.Endereco,
                PetNome = pet.Nome,
                PetRaca = pet.Raca,
                Porte = pet.Porte,
                Plano = model.Plano,
                BanhosRestantes = model.Plano == "Mensal" ? 4 : 2,
                Taxidog = model.Taxidog == "sim",
                Tosa = model.Tosa == "sim",
                PetRacao = pet.Racao,
                PetVermifugacao = pet.Vermifugacao,
                PetAntipulgas = pet.Antipulgas,
                PetVacinacao = pet.Vacinacao,
                Valor = model.Valor,
                DiasBanho = diasBanhoStr,
                DatasBanho = datasBanhoStr,
                Cpf = model.Responsavel.Cpf,
                Email = model.Responsavel.Email,
                PetIdade = pet.Idade
            };
            _context.Clientes.Add(cliente);

            var petEntity = new Pet
            {
                Nome = pet.Nome,
                Raca = pet.Raca,
                Tutor = model.Responsavel.Nome,
                Porte = pet.Porte,
                Racao = pet.Racao,
                Vermifugacao = pet.Vermifugacao,
                Antipulgas = pet.Antipulgas,
                Vacinacao = pet.Vacinacao,
                Idade = pet.Idade
            };
            _context.Pets.Add(petEntity);
        }

        await _context.SaveChangesAsync();

        if (model.DatasBanho != null && model.DatasBanho.Count > 0)
        {
            foreach (var pet in model.Pets)
            {
                foreach (var data in model.DatasBanho)
                {
                    var existe = await _context.Agendamentos.AnyAsync(a =>
                        a.Data == data && a.Pet == pet.Nome && a.Tutor == model.Responsavel.Nome);

                    if (!existe)
                    {
                        _context.Agendamentos.Add(new Agendamento
                        {
                            Data = data,
                            Horario = model.HorarioBanho ?? "08:00",
                            Pet = pet.Nome,
                            Tutor = model.Responsavel.Nome,
                            Plano = model.Plano,
                            Status = "Em espera"
                        });
                    }
                }
            }
            await _context.SaveChangesAsync();
        }

        return "Cliente cadastrado com sucesso!";
    }
}
