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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remarcar(int id, string novaData, string novoHorario)
    {
        await _agendamentoService.RemarcarAsync(id, novaData, novoHorario);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BanhoUnico(string tutor, string endereco, string telefone, string petNome, string data, string horario)
    {
        await _agendamentoService.CriarBanhoUnicoAsync(tutor, endereco, telefone, petNome, data, horario);
        return Ok(new { message = "Banho único agendado com sucesso!" });
    }
}
