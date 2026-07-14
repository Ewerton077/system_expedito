namespace SantoExpedito.DTOs;

public class CadastroViewModel
{
    public ResponsavelViewModel Responsavel { get; set; } = new();
    public List<PetViewModel> Pets { get; set; } = new();
    public string Plano { get; set; } = "Mensal";
    public string Taxidog { get; set; } = "sim";
    public string Tosa { get; set; } = "sim";
    public decimal Valor { get; set; }
    public List<string> DatasBanho { get; set; } = new();
}

public class ResponsavelViewModel
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Celular { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Cep { get; set; }
}

public class PetViewModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Idade { get; set; }
    public string Raca { get; set; } = string.Empty;
    public string Porte { get; set; } = string.Empty;
    public string? Racao { get; set; }
    public string? Vermifugacao { get; set; }
    public string? Antipulgas { get; set; }
    public string? Vacinacao { get; set; }
    public string Medicamento { get; set; } = "nao";
    public string? MedicamentoNome { get; set; }
}
