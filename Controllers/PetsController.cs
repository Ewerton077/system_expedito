using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Controllers;

[Authorize]
public class PetsController : Controller
{
    private readonly IPetService _petService;
    private readonly IClienteService _clienteService;
    private readonly IAgendamentoService _agendamentoService;

    public PetsController(IPetService petService, IClienteService clienteService, IAgendamentoService agendamentoService)
    {
        _petService = petService;
        _clienteService = clienteService;
        _agendamentoService = agendamentoService;
    }

    public async Task<IActionResult> Listar()
    {
        var pets = await _petService.GetAllAsync();
        return Json(pets.Select(p => new { p.Id, p.Nome, p.Tutor }));
    }

    public async Task<IActionResult> Index(int? detalheId, string? search)
    {
        var pets = await _petService.GetAllAsync(search);
        var clientes = await _clienteService.GetAllAsync();
        ViewBag.Clientes = clientes;
        ViewBag.SearchTerm = search;
        ViewBag.PetDetalhe = detalheId.HasValue ? pets.FirstOrDefault(p => p.Id == detalheId.Value) : null;

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

        return View(pets);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Excluir(int id)
    {
        await _petService.DeleteAsync(id);
        return RedirectToAction("Index");
    }
}
