using System.ComponentModel.DataAnnotations;

namespace SantoExpedito.Models;

public class Pet
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Raca { get; set; }

    [MaxLength(100)]
    public string? Tutor { get; set; }

    [MaxLength(20)]
    public string? Porte { get; set; }

    [MaxLength(100)]
    public string? Racao { get; set; }

    [MaxLength(20)]
    public string? Vermifugacao { get; set; }

    [MaxLength(20)]
    public string? Antipulgas { get; set; }

    [MaxLength(20)]
    public string? Vacinacao { get; set; }

    [MaxLength(20)]
    public string? Idade { get; set; }

    public bool Medicamento { get; set; }

    [MaxLength(100)]
    public string? MedicamentoNome { get; set; }
}
