namespace SantoExpedito.Services.RegrasAgendamento;

public interface IRegraAgendamento
{
    string Nome { get; }
    DateTime Aplicar(DateTime data);
}
