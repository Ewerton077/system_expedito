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
    private readonly IAgendamentoService _agendamentoService;
    private readonly IVencimentoService _vencimentoService;

    public ClientesController(IClienteService clienteService, IPetService petService, IAgendamentoService agendamentoService, IVencimentoService vencimentoService)
    {
        _clienteService = clienteService;
        _petService = petService;
        _agendamentoService = agendamentoService;
        _vencimentoService = vencimentoService;
    }

    public async Task<IActionResult> Index(string? plano, int? detalheId, int? editId, string? search)
    {
        var clientes = await _clienteService.GetAllAsync(search);
        ViewBag.PlanoAtivo = plano ?? "Mensal";
        ViewBag.SearchTerm = search;
        ViewBag.Clientes = clientes;

        var agora = DateTime.Now;
        var todosAgendamentos = await _agendamentoService.GetAllAsync();
        var banhosRestantesDict = new Dictionary<int, int>();
        foreach (var c in clientes)
        {
            var banhosPorMes = c.Plano == "Mensal" ? 4 : 2;
            var concluidos = todosAgendamentos.Count(a =>
                a.Status == "Concluído" &&
                a.Tutor == c.Nome &&
                a.Pet == c.PetNome &&
                !string.IsNullOrEmpty(a.Data) &&
                a.Data.EndsWith($"/{agora.Month:D2}/{agora.Year}"));
            banhosRestantesDict[c.Id] = Math.Max(0, banhosPorMes - concluidos);
        }
        ViewBag.BanhosRestantesDict = banhosRestantesDict;

        if (editId.HasValue)
            ViewBag.ClienteEdit = clientes.FirstOrDefault(c => c.Id == editId.Value);
        else if (detalheId.HasValue)
        {
            var clienteDetalhe = clientes.FirstOrDefault(c => c.Id == detalheId.Value);
            ViewBag.ClienteDetalhe = clienteDetalhe;
            if (clienteDetalhe != null)
            {
                var vencimentoAtual = await _vencimentoService.GetVencimentoClienteAsync(clienteDetalhe.Nome);
                if (vencimentoAtual != null)
                {
                    ViewBag.DataInicioVenc = vencimentoAtual.DataInicio;
                    var partes = (vencimentoAtual.VencimentoData ?? "").Split('/');
                    if (partes.Length == 3 && DateTime.TryParse($"{partes[2]}-{partes[1]}-{partes[0]}", out var dataVenc))
                    {
                        var diasRestantes = (dataVenc.Date - DateTime.Now.Date).Days;
                        ViewBag.DataVencimento = dataVenc;
                        ViewBag.DiasRestantes = diasRestantes;
                        ViewBag.PlanoVencido = diasRestantes < 0;
                    }
                    else
                    {
                        ViewBag.PlanoVencido = false;
                    }
                }
                if (!string.IsNullOrWhiteSpace(clienteDetalhe.DatasBanho))
                {
                    var datasProximos = _agendamentoService.ObterDatasProximosBanhos(
                        clienteDetalhe.Plano ?? "Mensal", clienteDetalhe.DatasBanho);
                    ViewBag.ProximosBanhos = datasProximos;
                }
            }
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Salvar(int id, string nome, string? telefone, string? endereco,
        string? petNome, string? petRaca, string? porte, string? plano,
        int banhosRestantes, bool taxidog, bool tosa, decimal valor,
        string? petRacao, string? petVermifugacao, string? petAntipulgas, string? petVacinacao,
        string? cpf, string? email, string? petIdade, string? cep,
        string? datasBanho)
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
                Cpf = cpf, Email = email, PetIdade = petIdade, Cep = cep,
                DatasBanho = datasBanho,
                DataCadastro = DateTime.Now
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
            existing.Cpf = cpf; existing.Email = email; existing.PetIdade = petIdade; existing.Cep = cep;
            existing.DatasBanho = datasBanho;

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GerarMock()
    {
        var msg = await _clienteService.GerarMockDataAsync();
        TempData["MockMessage"] = msg;
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> LimparMock()
    {
        var count = await _clienteService.LimparMockDataAsync();
        if (count > 0)
            TempData["MockMessage"] = $"Removidos {count} clientes mock.";
        else
            TempData["MockMessage"] = "Nenhum cliente mock encontrado.";
        return RedirectToAction("Index");
    }
}
