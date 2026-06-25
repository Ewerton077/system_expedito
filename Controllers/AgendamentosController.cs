using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Controllers;

[Authorize]
public class AgendamentosController : Controller
{
    private readonly IAgendamentoService _agendamentoService;

    public AgendamentosController(IAgendamentoService agendamentoService)
    {
        _agendamentoService = agendamentoService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Listar()
    {
        var agendamentos = await _agendamentoService.GetAllAsync();
        return Json(agendamentos);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AtualizarStatus(int id, string status)
    {
        await _agendamentoService.UpdateStatusAsync(id, status);
        return RedirectToAction("Index");
    }
}
