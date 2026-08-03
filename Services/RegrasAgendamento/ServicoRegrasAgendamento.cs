namespace SantoExpedito.Services.RegrasAgendamento;

public class ServicoRegrasAgendamento
{
    private readonly List<IRegraAgendamento> _regras;

    public ServicoRegrasAgendamento(IEnumerable<IRegraAgendamento> regras)
    {
        _regras = regras.ToList();
    }

    public DateTime Aplicar(DateTime data)
    {
        foreach (var regra in _regras)
            data = regra.Aplicar(data);
        return data;
    }

    public IReadOnlyList<IRegraAgendamento> Regras => _regras.AsReadOnly();
}
