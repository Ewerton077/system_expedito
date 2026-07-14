using System.ComponentModel.DataAnnotations;

namespace SantoExpedito.Models;

public class Agendamento
{
    [Key]
    public int Id { get; set; }

    [MaxLength(20)]
    public string? Data { get; set; }

    [MaxLength(10)]
    public string? Horario { get; set; }

    [MaxLength(100)]
    public string? Pet { get; set; }

    [MaxLength(100)]
    public string? Tutor { get; set; }

    [MaxLength(200)]
    public string? Endereco { get; set; }

    [MaxLength(20)]
    public string? Telefone { get; set; }

    [MaxLength(20)]
    public string? Plano { get; set; }

    [MaxLength(20)]
    public string? Status { get; set; }

}
