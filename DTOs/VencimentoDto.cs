namespace SantoExpedito.DTOs;

public class VencimentoListDto
{
    public List<VencimentoItemDto> Todos { get; set; } = new();
    public List<VencimentoItemDto> Pagos { get; set; } = new();
    public List<VencimentoItemDto> Pendentes { get; set; } = new();
    public List<VencimentoItemDto> Atrasados { get; set; } = new();
    public List<VencimentoItemDto> VencemHoje { get; set; } = new();
    public List<VencimentoItemDto> VencemProximos7Dias { get; set; } = new();
}

public class VencimentoItemDto
{
    public int Id { get; set; }
    public string? Cliente { get; set; }
    public string? Plano { get; set; }
    public string? DataInicio { get; set; }
    public string? VencimentoData { get; set; }
    public decimal Valor { get; set; }
    public string? Status { get; set; }
}

public class FinanceiroDto
{
    public decimal ReceitaTotal { get; set; }
    public decimal ReceitaMensal { get; set; }
    public decimal ReceitaQuinzenal { get; set; }
    public int ClientesMensal { get; set; }
    public int ClientesQuinzenal { get; set; }
}
