using SantoExpedito.Models;

namespace SantoExpedito.DTOs;

public class RelatorioDto
{
    public int TotalClientes { get; set; }
    public int ClientesMensal { get; set; }
    public int ClientesQuinzenal { get; set; }
    public int TotalPets { get; set; }
    public int PetsPequeno { get; set; }
    public int PetsMedio { get; set; }
    public int PetsGrande { get; set; }
    public decimal ReceitaMensal { get; set; }
    public decimal ReceitaQuinzenal { get; set; }
    public decimal ReceitaTotal { get; set; }
    public List<Cliente> Clientes { get; set; } = new();
    public List<Pet> Pets { get; set; } = new();
}
