using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantoExpedito.DTOs;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Controllers;

[Authorize]
public class CadastroController : Controller
{
    private readonly ICadastroService _cadastroService;

    public CadastroController(ICadastroService cadastroService)
    {
        _cadastroService = cadastroService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Salvar([FromBody] CadastroViewModel model)
    {
        var result = await _cadastroService.CreateClienteAsync(model);
        if (result.StartsWith("Adicione"))
            return BadRequest(result);
        return Ok(new { message = result });
    }
}
