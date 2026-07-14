using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SantoExpedito.Models;

public class Cliente
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Telefone { get; set; }

    [MaxLength(200)]
    public string? Endereco { get; set; }

    [MaxLength(100)]
    public string? PetNome { get; set; }

    [MaxLength(100)]
    public string? PetRaca { get; set; }

    [MaxLength(20)]
    public string? Porte { get; set; }

    [MaxLength(20)]
    public string? Plano { get; set; }

    public int BanhosRestantes { get; set; }

    public bool Taxidog { get; set; }

    public bool Tosa { get; set; }

    [MaxLength(100)]
    public string? PetRacao { get; set; }

    [MaxLength(20)]
    public string? PetVermifugacao { get; set; }

    [MaxLength(20)]
    public string? PetAntipulgas { get; set; }

    [MaxLength(20)]
    public string? PetVacinacao { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Valor { get; set; }

    public string? DatasBanho { get; set; }

    [MaxLength(20)]
    public string? PetIdade { get; set; }

    [MaxLength(14)]
    public string? Cpf { get; set; }

    [MaxLength(10)]
    public string? Cep { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    public DateTime? DataCadastro { get; set; }

    public List<string> ObterDatasBanho()
    {
        if (string.IsNullOrEmpty(DatasBanho)) return new List<string>();
        return DatasBanho.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public void DefinirDatasBanho(List<string> datas)
    {
        DatasBanho = string.Join(",", datas);
    }
}
