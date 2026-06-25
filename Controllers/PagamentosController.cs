using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Controllers;

[Authorize]
public class PagamentosController : Controller
{
    private readonly IVencimentoService _vencimentoService;

    public PagamentosController(IVencimentoService vencimentoService)
    {
        _vencimentoService = vencimentoService;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _vencimentoService.GetVencimentosAsync();
        ViewBag.Pagos = data.Pagos;
        ViewBag.Pendentes = data.Pendentes;
        ViewBag.Financeiro = await _vencimentoService.GetFinanceiroAsync();
        return View();
    }
}
