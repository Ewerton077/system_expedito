using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SantoExpedito.Data;
using SantoExpedito.Services;
using SantoExpedito.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Login/Logout";
        options.AccessDeniedPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();
builder.Services.AddScoped<IVencimentoService, VencimentoService>();
builder.Services.AddScoped<ICadastroService, CadastroService>();

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
        context.AdminUsers.Add(new SantoExpedito.Models.AdminUser
        {
            Username = "root",
            PasswordHash = BCryptHelper.HashPassword("krakermanfodase")
        });
        context.AdminUsers.Add(new SantoExpedito.Models.AdminUser
        {
            Username = "evin",
            PasswordHash = BCryptHelper.HashPassword("1988tupac")
        });
        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
