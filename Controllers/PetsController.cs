using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Controllers;

[Authorize]
public class PetsController : Controller
{
    private readonly IPetService _petService;
    private readonly IClienteService _clienteService;

    public PetsController(IPetService petService, IClienteService clienteService)
    {
        _petService = petService;
        _clienteService = clienteService;
    }

    public async Task<IActionResult> Index(int? detalheId)
    {
        var pets = await _petService.GetAllAsync();
        var clientes = await _clienteService.GetAllAsync();
        ViewBag.Clientes = clientes;
        ViewBag.PetDetalhe = detalheId.HasValue ? pets.FirstOrDefault(p => p.Id == detalheId.Value) : null;
        return View(pets);
    }
}
