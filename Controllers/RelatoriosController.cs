using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Controllers;

[Authorize]
public class RelatoriosController : Controller
{
    private readonly IClienteService _clienteService;
    private readonly IPetService _petService;

    public RelatoriosController(IClienteService clienteService, IPetService petService)
    {
        _clienteService = clienteService;
        _petService = petService;
    }

    public async Task<IActionResult> Index()
    {
        var clientes = await _clienteService.GetAllAsync();
        var pets = await _petService.GetAllAsync();

        ViewBag.TotalClientes = clientes.Count;
        ViewBag.ClientesMensal = clientes.Count(c => c.Plano == "Mensal");
        ViewBag.ClientesQuinzenal = clientes.Count(c => c.Plano == "Quinzenal");
        ViewBag.TotalPets = pets.Count;
        ViewBag.PetsPequeno = pets.Count(p => p.Porte == "Pequeno");
        ViewBag.PetsMedio = pets.Count(p => p.Porte == "Médio");
        ViewBag.PetsGrande = pets.Count(p => p.Porte == "Grande");
        ViewBag.ReceitaMensal = clientes.Where(c => c.Plano == "Mensal").Sum(c => c.Valor);
        ViewBag.ReceitaQuinzenal = clientes.Where(c => c.Plano == "Quinzenal").Sum(c => c.Valor);
        ViewBag.ReceitaTotal = clientes.Sum(c => c.Valor);
        ViewBag.Clientes = clientes;
        ViewBag.Pets = pets;

        return View();
    }
}
