using System.Security.Cryptography;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SantoExpedito.Data;
using SantoExpedito.Models;
using SantoExpedito.Services;
using SantoExpedito.Services.Interfaces;
using SantoExpedito.Services.RegrasAgendamento;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = null;
});

builder.Services.Configure<HostFilteringOptions>(options =>
{
    options.AllowedHosts.Clear();
    options.AllowedHosts.Add("*");
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? throw new InvalidOperationException("Connection string não configurada");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Login/Logout";
        options.AccessDeniedPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

builder.Services.AddAuthorization();

builder.Services.AddMemoryCache();
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User?.Identity?.IsAuthenticated == true ? httpContext.User.Identity.Name : httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});

builder.Services.AddScoped<ILogService, LogService>();

builder.Services.AddScoped<ILoginAttemptService, LoginAttemptService>();

builder.Services.AddScoped<ICpfEncryptionService, CpfEncryptionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();
builder.Services.AddScoped<IVencimentoService, VencimentoService>();
builder.Services.AddScoped<ICadastroService, CadastroService>();
builder.Services.AddScoped<IRegraAgendamento, RegraDomingo>();
builder.Services.AddScoped<ServicoRegrasAgendamento>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    var mockClientes = context.Clientes.Where(c => c.Nome.StartsWith("[MOCK]")).ToList();
    if (mockClientes.Count > 0)
    {
        var mockNomes = mockClientes.Select(c => c.Nome).ToHashSet();
        context.Agendamentos.RemoveRange(context.Agendamentos.Where(a => mockNomes.Contains(a.Tutor ?? "")));
        context.Vencimentos.RemoveRange(context.Vencimentos.Where(v => mockNomes.Contains(v.Cliente ?? "")));
        context.Pets.RemoveRange(context.Pets.Where(p => mockNomes.Contains(p.Tutor ?? "")));
        context.Clientes.RemoveRange(mockClientes);
        context.SaveChanges();
    }

    if (!context.AdminUsers.Any())
    {
        var senhaCasaderacao = Environment.GetEnvironmentVariable("ADMIN_PASSWORD_CASADERACAO")
            ?? builder.Configuration["AdminPasswords:Casaderacao"]
            ?? throw new InvalidOperationException("ADMIN_PASSWORD_CASADERACAO não configurada");

        var senhaEvin = Environment.GetEnvironmentVariable("ADMIN_PASSWORD_EVIN")
            ?? builder.Configuration["AdminPasswords:Evin"]
            ?? throw new InvalidOperationException("ADMIN_PASSWORD_EVIN não configurada");

        context.AdminUsers.Add(new AdminUser
        {
            Username = "casaderacao",
            PasswordHash = BCryptHelper.HashPassword(senhaCasaderacao),
            Role = UserRole.Admin
        });
        context.AdminUsers.Add(new AdminUser
        {
            Username = "evin",
            PasswordHash = BCryptHelper.HashPassword(senhaEvin),
            Role = UserRole.SuperAdmin
        });
        context.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    ctx.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    await next();
});

app.UseRateLimiter();
app.UseHostFiltering();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logService = context.RequestServices.GetRequiredService<ILogService>();
        await logService.RegistrarAsync("Error", ex.Message, ex.StackTrace, context.User?.Identity?.Name, context.Request.Path);
        throw;
    }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
