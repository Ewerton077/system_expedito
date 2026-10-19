using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class LogsController : Controller
{
    private readonly ILogService _logService;

    public LogsController(ILogService logService)
    {
        _logService = logService;
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _logService.ObterTodosAsync();
        return View(logs);
    }
}
