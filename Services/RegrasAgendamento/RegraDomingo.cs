namespace SantoExpedito.Services.RegrasAgendamento;

public class RegraDomingo : IRegraAgendamento
{
    public string Nome => "Ajuste Domingo → Segunda";

    public DateTime Aplicar(DateTime data)
    {
        if (data.DayOfWeek == DayOfWeek.Sunday)
            return data.AddDays(1);
        return data;
    }
}
