using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantoExpedito.Models;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Controllers;

[Authorize]
public class ClientesController : Controller
{
    private readonly IClienteService _clienteService;
    private readonly IPetService _petService;

    public ClientesController(IClienteService clienteService, IPetService petService)
    {
        _clienteService = clienteService;
        _petService = petService;
    }

    public async Task<IActionResult> Index(string? plano, int? detalheId, int? editId)
    {
        var clientes = await _clienteService.GetAllAsync();
        ViewBag.PlanoAtivo = plano ?? "Mensal";
        ViewBag.Clientes = clientes;

        if (editId.HasValue)
            ViewBag.ClienteEdit = clientes.FirstOrDefault(c => c.Id == editId.Value);
        else if (detalheId.HasValue)
            ViewBag.ClienteDetalhe = clientes.FirstOrDefault(c => c.Id == detalheId.Value);

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Salvar(int id, string nome, string? telefone, string? endereco,
        string? petNome, string? petRaca, string? porte, string? plano,
        int banhosRestantes, bool taxidog, bool tosa, decimal valor,
        string? petRacao, string? petVermifugacao, string? petAntipulgas, string? petVacinacao,
        string? cpf, string? email, string? petIdade,
        string? diasBanho, string? datasBanho)
    {
        if (id == 0)
        {
            await _clienteService.CreateAsync(new Cliente
            {
                Nome = nome, Telefone = telefone, Endereco = endereco,
                PetNome = petNome, PetRaca = petRaca, Porte = porte, Plano = plano,
                BanhosRestantes = banhosRestantes, Taxidog = taxidog, Tosa = tosa, Valor = valor,
                PetRacao = petRacao, PetVermifugacao = petVermifugacao,
                PetAntipulgas = petAntipulgas, PetVacinacao = petVacinacao,
                Cpf = cpf, Email = email, PetIdade = petIdade,
                DiasBanho = diasBanho, DatasBanho = datasBanho
            });
        }
        else
        {
            var existing = await _clienteService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.Nome = nome; existing.Telefone = telefone; existing.Endereco = endereco;
            existing.PetNome = petNome; existing.PetRaca = petRaca; existing.Porte = porte;
            existing.Plano = plano; existing.BanhosRestantes = banhosRestantes;
            existing.Taxidog = taxidog; existing.Tosa = tosa; existing.Valor = valor;
            existing.PetRacao = petRacao; existing.PetVermifugacao = petVermifugacao;
            existing.PetAntipulgas = petAntipulgas; existing.PetVacinacao = petVacinacao;
            existing.Cpf = cpf; existing.Email = email; existing.PetIdade = petIdade;
            existing.DiasBanho = diasBanho; existing.DatasBanho = datasBanho;

            await _clienteService.UpdateAsync(existing);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(int id)
    {
        await _clienteService.DeleteAsync(id);
        return RedirectToAction("Index");
    }
}
