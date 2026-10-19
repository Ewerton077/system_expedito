using Microsoft.EntityFrameworkCore;
using SantoExpedito.Data;
using SantoExpedito.Models;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Services;

public class LogService : ILogService
{
    private readonly AppDbContext _context;

    public LogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task RegistrarAsync(string nivel, string mensagem, string? detalhes = null, string? usuario = null, string? endpoint = null)
    {
        var log = new Log
        {
            Nivel = nivel,
            Mensagem = mensagem,
            Detalhes = detalhes,
            Usuario = usuario,
            Endpoint = endpoint,
            Data = DateTime.UtcNow
        };

        _context.Logs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Log>> ObterTodosAsync()
    {
        return await _context.Logs.OrderByDescending(l => l.Data).Take(500).ToListAsync();
    }

    public async Task<List<Log>> ObterPorNivelAsync(string nivel)
    {
        return await _context.Logs.Where(l => l.Nivel == nivel).OrderByDescending(l => l.Data).Take(500).ToListAsync();
    }
}
