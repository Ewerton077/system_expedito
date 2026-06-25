using Microsoft.EntityFrameworkCore;
using SantoExpedito.Data;
using SantoExpedito.Models;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Services;

public class AgendamentoService : IAgendamentoService
{
    private readonly AppDbContext _context;

    public AgendamentoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Agendamento>> GetAllAsync()
    {
        return await _context.Agendamentos.OrderBy(a => a.Data).ThenBy(a => a.Horario).ToListAsync();
    }

    public async Task UpdateStatusAsync(int id, string status)
    {
        var agendamento = await _context.Agendamentos.FindAsync(id);
        if (agendamento != null)
        {
            agendamento.Status = status;
            await _context.SaveChangesAsync();
        }
    }
}
