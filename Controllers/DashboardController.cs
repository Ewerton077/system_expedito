using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _dashboardService.GetDashboardDataAsync();

        ViewBag.TotalClientes = data.TotalClientes;
        ViewBag.TotalPets = data.TotalPets;
        ViewBag.PlanosMensais = data.PlanosMensais;
        ViewBag.PlanosQuinzenais = data.PlanosQuinzenais;
        ViewBag.PagamentosPagos = data.PagamentosPagos;
        ViewBag.PagamentosPendentes = data.PagamentosPendentes;
        ViewBag.PagamentosAtrasados = data.PagamentosAtrasados;
        ViewBag.AgendamentosHoje = data.AgendamentosHoje;
        ViewBag.Vencimentos = data.Vencimentos;
        ViewBag.DbSizeMb = data.DbSizeMb;
        ViewBag.DbResponseTimeMs = data.DbResponseTimeMs;

        return View();
    }
}
