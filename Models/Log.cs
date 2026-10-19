using System.ComponentModel.DataAnnotations;

namespace SantoExpedito.Models;

public class Log
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Nivel { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Mensagem { get; set; } = string.Empty;

    public string? Detalhes { get; set; }

    public DateTime Data { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? Usuario { get; set; }

    [MaxLength(200)]
    public string? Endpoint { get; set; }
}
