using SantoExpedito.Models;

namespace SantoExpedito.Services.Interfaces;

public interface IAgendamentoService
{
    Task<List<Agendamento>> GetAllAsync();
    Task UpdateStatusAsync(int id, string status);
}
