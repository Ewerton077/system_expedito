using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SantoExpedito.Models;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Controllers;

public class LoginController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILoginAttemptService _loginAttemptService;

    public LoginController(IAuthService authService, ILoginAttemptService loginAttemptService)
    {
        _authService = authService;
        _loginAttemptService = loginAttemptService;
    }

    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (_loginAttemptService.IsBlocked(ipAddress))
        {
            ModelState.AddModelError("", "Muitas tentativas de login. Tente novamente em 5 minutos.");
            return View(model);
        }

        if (!ModelState.IsValid)
            return View(model);

        if (!await _authService.ValidateUserAsync(model.Username, model.Password))
        {
            _loginAttemptService.RecordFailedAttempt(ipAddress);
            ModelState.AddModelError("", "Usuário ou senha inválidos");
            return View(model);
        }

        _loginAttemptService.Reset(ipAddress);
        var principal = await _authService.CreatePrincipalAsync(model.Username);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Dashboard");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index");
    }
}
