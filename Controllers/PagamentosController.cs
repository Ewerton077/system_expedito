using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Controllers;

[Authorize]
public class PagamentosController : Controller
{
    private readonly IVencimentoService _vencimentoService;
    private readonly IClienteService _clienteService;

    public PagamentosController(IVencimentoService vencimentoService, IClienteService clienteService)
    {
        _vencimentoService = vencimentoService;
        _clienteService = clienteService;
    }

    public async Task<IActionResult> Index(string? filtro)
    {
        var data = await _vencimentoService.GetVencimentosAsync();
        var filtroAtivo = filtro ?? "todos";

        var vencimentosFiltrados = filtroAtivo switch
        {
            "pagos" => data.Pagos,
            "pendentes" => data.Pendentes,
            "atrasados" => data.Atrasados,
            "hoje" => data.VencemHoje,
            "proximos7" => data.VencemProximos7Dias,
            _ => data.Todos
        };

        ViewBag.Vencimentos = vencimentosFiltrados;
        ViewBag.FiltroAtivo = filtroAtivo;
        ViewBag.ContadorPagos = data.Pagos.Count;
        ViewBag.ContadorPendentes = data.Pendentes.Count;
        ViewBag.ContadorAtrasados = data.Atrasados.Count;
        ViewBag.ContadorHoje = data.VencemHoje.Count;
        ViewBag.ContadorProx7 = data.VencemProximos7Dias.Count;
        ViewBag.ContadorTodos = data.Todos.Count;
        ViewBag.Financeiro = await _vencimentoService.GetFinanceiroAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AlterarStatus(int id, string status)
    {
        await _vencimentoService.AlterarStatusAsync(id, status);
        return Ok(new { message = $"Status alterado para {status}." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RenovarPlano(int id)
    {
        await _vencimentoService.RenovarPlanoAsync(id);
        return Ok(new { message = "Plano renovado com sucesso!" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LimparOrfaos()
    {
        var removidos = await _vencimentoService.RemoverOrfaosAsync();
        return Ok(new { message = $"{removidos} pagamento(s) órfão(s) removido(s).", removidos });
    }
}
