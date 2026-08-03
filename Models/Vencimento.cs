using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SantoExpedito.Models;

public class Vencimento
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string? Cliente { get; set; }

    [MaxLength(20)]
    public string? Plano { get; set; }

    [MaxLength(20)]
    public string? DataInicio { get; set; }

    [MaxLength(20)]
    public string? VencimentoData { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Valor { get; set; }

    [MaxLength(20)]
    public string? Status { get; set; }
}
