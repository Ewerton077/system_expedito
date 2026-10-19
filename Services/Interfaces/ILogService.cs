using SantoExpedito.Data;
using SantoExpedito.Models;

namespace SantoExpedito.Services.Interfaces;

public interface ILogService
{
    Task RegistrarAsync(string nivel, string mensagem, string? detalhes = null, string? usuario = null, string? endpoint = null);
    Task<List<Log>> ObterTodosAsync();
    Task<List<Log>> ObterPorNivelAsync(string nivel);
}
