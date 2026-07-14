using SantoExpedito.Models;

namespace SantoExpedito.Services.Interfaces;

public interface IAgendamentoService
{
    Task<List<Agendamento>> GetAllAsync();
    Task UpdateStatusAsync(int id, string status);
    Task RemarcarAsync(int id, string novaData, string novoHorario);
    Task GerarAgendamentosRecorrentesAsync(string tutor, string petNome, string plano, string datasBanho, int meses = 12);
    Task CriarBanhoUnicoAsync(string tutor, string endereco, string telefone, string petNome, string data, string horario);
}
