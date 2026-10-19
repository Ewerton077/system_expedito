using Microsoft.EntityFrameworkCore;
using SantoExpedito.Models;

namespace SantoExpedito.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Log> Logs { get; set; } = null!;

    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Pet> Pets { get; set; } = null!;
    public DbSet<Agendamento> Agendamentos { get; set; } = null!;
    public DbSet<Vencimento> Vencimentos { get; set; } = null!;
    public DbSet<AdminUser> AdminUsers { get; set; } = null!;
}
