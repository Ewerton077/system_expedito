using System.ComponentModel.DataAnnotations;

namespace SantoExpedito.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Usuário é obrigatório")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
