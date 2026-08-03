using Microsoft.EntityFrameworkCore;
using SantoExpedito.Data;
using SantoExpedito.DTOs;
using SantoExpedito.Models;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Services;

public class CadastroService : ICadastroService
{
    private readonly AppDbContext _context;
    private readonly IAgendamentoService _agendamentoService;

    public CadastroService(AppDbContext context, IAgendamentoService agendamentoService)
    {
        _context = context;
        _agendamentoService = agendamentoService;
    }

    public async Task<string> CreateClienteAsync(CadastroViewModel model)
    {
        if (model.Pets == null || model.Pets.Count == 0)
            return "Adicione ao menos um pet";

        var stored = await _context.Clientes.ToListAsync();
        int maxId = stored.Count > 0 ? stored.Max(c => c.Id) : 0;

        var diasBanho = model.DatasBanho
            .Select(d => d.Split('/')[0])
            .ToList();
        var datasBanhoStr = string.Join(",", diasBanho);

        var hoje = DateTime.Now;
        var datasVencStr = VencimentoService.CalcularVencimento(hoje, model.Plano);
        var hojeStr = $"{hoje.Day:D2}/{hoje.Month:D2}/{hoje.Year}";

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
                DatasBanho = datasBanhoStr,
                Cpf = model.Responsavel.Cpf,
                Email = model.Responsavel.Email,
                Cep = model.Responsavel.Cep,
                PetIdade = pet.Idade,
                DataCadastro = DateTime.Now
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
                Idade = pet.Idade,
                Medicamento = pet.Medicamento == "sim",
                MedicamentoNome = pet.Medicamento == "sim" ? pet.MedicamentoNome : null
            };
            _context.Pets.Add(petEntity);
        }

        await _context.SaveChangesAsync();

        var vencimento = new Vencimento
        {
            Cliente = model.Responsavel.Nome,
            Plano = model.Plano,
            DataInicio = hojeStr,
            VencimentoData = datasVencStr,
            Valor = model.Valor,
            Status = "Pendente"
        };
        _context.Vencimentos.Add(vencimento);
        await _context.SaveChangesAsync();

        if (model.DatasBanho != null && model.DatasBanho.Count > 0)
        {
            foreach (var pet in model.Pets)
            {
                await _agendamentoService.GerarAgendamentosRecorrentesAsync(
                    model.Responsavel.Nome, pet.Nome, model.Plano, datasBanhoStr, 12);
            }
        }

        return "Cliente cadastrado com sucesso!";
    }
}
