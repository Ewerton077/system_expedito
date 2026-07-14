using Microsoft.EntityFrameworkCore;
using SantoExpedito.Data;
using SantoExpedito.Models;
using SantoExpedito.Services.Interfaces;

namespace SantoExpedito.Services;

public class ClienteService : IClienteService
{
    private readonly AppDbContext _context;
    private readonly IAgendamentoService _agendamentoService;

    public ClienteService(AppDbContext context, IAgendamentoService agendamentoService)
    {
        _context = context;
        _agendamentoService = agendamentoService;
    }

    public async Task<List<Cliente>> GetAllAsync(string? searchTerm = null)
    {
        var query = _context.Clientes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(c =>
                c.Nome.ToLower().Contains(term) ||
                (c.PetNome != null && c.PetNome.ToLower().Contains(term)) ||
                (c.Telefone != null && c.Telefone.Contains(term)) ||
                (c.Email != null && c.Email.ToLower().Contains(term)) ||
                (c.PetRaca != null && c.PetRaca.ToLower().Contains(term))
            );
        }

        return await query.OrderBy(c => c.Nome).ToListAsync();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _context.Clientes.FindAsync(id);
    }

    public async Task<Cliente> CreateAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        var existing = await _context.Clientes.FindAsync(cliente.Id);
        if (existing == null) return;

        existing.Nome = cliente.Nome;
        existing.Telefone = cliente.Telefone;
        existing.Endereco = cliente.Endereco;
        existing.PetNome = cliente.PetNome;
        existing.PetRaca = cliente.PetRaca;
        existing.Porte = cliente.Porte;
        existing.Plano = cliente.Plano;
        existing.BanhosRestantes = cliente.BanhosRestantes;
        existing.Taxidog = cliente.Taxidog;
        existing.Tosa = cliente.Tosa;
        existing.Valor = cliente.Valor;
        existing.DatasBanho = cliente.DatasBanho;
        existing.PetRacao = cliente.PetRacao;
        existing.PetVermifugacao = cliente.PetVermifugacao;
        existing.PetAntipulgas = cliente.PetAntipulgas;
        existing.PetVacinacao = cliente.PetVacinacao;
        existing.Cpf = cliente.Cpf;
        existing.Email = cliente.Email;
        existing.Cep = cliente.Cep;
        existing.PetIdade = cliente.PetIdade;

        await UpdatePetAsync(existing);
        await _context.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(existing.DatasBanho) && !string.IsNullOrWhiteSpace(existing.PetNome))
        {
            await _agendamentoService.GerarAgendamentosRecorrentesAsync(
                existing.Nome, existing.PetNome, existing.Plano ?? "Mensal", existing.DatasBanho, 12);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente != null)
        {
            var pets = await _context.Pets.Where(p => p.Nome == cliente.PetNome && p.Tutor == cliente.Nome).ToListAsync();
            _context.Pets.RemoveRange(pets);
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdatePetAsync(Cliente cliente)
    {
        var pet = await _context.Pets.FirstOrDefaultAsync(p =>
            p.Nome == cliente.PetNome && p.Tutor == cliente.Nome);
        if (pet != null)
        {
            pet.Raca = cliente.PetRaca;
            pet.Porte = cliente.Porte;
            pet.Racao = cliente.PetRacao;
            pet.Vermifugacao = cliente.PetVermifugacao;
            pet.Antipulgas = cliente.PetAntipulgas;
            pet.Vacinacao = cliente.PetVacinacao;
            pet.Idade = cliente.PetIdade;
        }
    }

    public async Task<string> GerarMockDataAsync()
    {
        var rng = new Random();

        string[] nomes = {"Ana","Carlos","Maria","João","Paula","Pedro","Juliana","Lucas","Camila","Rafael",
            "Fernanda","Gabriel","Amanda","Marcos","Letícia","Thiago","Isabela","Felipe","Larissa","Bruno",
            "Natália","Eduardo","Renata","Diego","Vanessa","Leonardo","Tatiane","Gustavo","Bianca","Rodrigo",
            "Patrícia","Alexandre","Sabrina","André","Jéssica","Ricardo","Aline","Vinicius","Priscila","Fábio",
            "Débora","Matheus","Cristiane","Murilo","Sandra","Igor","Michele","César","Luciana","Wagner"};
        string[] sobrenomes = {"Silva","Santos","Oliveira","Souza","Lima","Pereira","Costa","Almeida","Nascimento",
            "Barbosa","Ribeiro","Martins","Carvalho","Gomes","Araújo","Melo","Cavalcanti","Dias","Moreira","Castro",
            "Ferreira","Correia","Mendes","Teixeira","Cardoso","Campos","Freitas","Monteiro","Fernandes","Barros"};
        string[] ruas = {"Rua das Flores","Av. Brasil","Rua dos Pinheiros","Rua XV de Novembro","Av. Paulista",
            "Rua da Praia","Rua Sete de Setembro","Av. Independência","Rua do Comércio","Rua Boa Vista",
            "Rua São João","Av. Central","Rua da Aurora","Rua do Imperador","Rua Marechal Deodoro"};
        string[] bairros = {"Centro","Jardim América","Vila Nova","Boa Vista","Santa Cecília","São Francisco",
            "Parque Industrial","Residencial Park","Cidade Jardim","Vila Operária"};
        string[] cidades = {"São Paulo","Rio de Janeiro","Belo Horizonte","Porto Alegre","Curitiba","Salvador",
            "Fortaleza","Brasília","Recife","Campinas"};
        string[] ufs = {"SP","RJ","MG","RS","PR","BA","CE","DF","PE","SP"};
        string[] racas = {"Vira-lata","Golden Retriever","Labrador","Pastor Alemão","Poodle","Buldogue Francês",
            "Shih Tzu","Yorkshire","Beagle","Boxer","Pug","Maltês","Lhasa Apso","Dachshund","Husky Siberiano"};
        string[] portes = {"Pequeno","Médio","Grande"};
        string[] racasPet = {"Golden Premier","Royal Canin","Whiskas","Pedigree","Special Dog","Premier Pet",
            "Friskies","Purina","Doguito","Nacional"};
        string[] planos = {"Mensal","Quinzenal"};
        string[] statusAgenda = {"Em espera","Em processo","Concluido"};

        string mockPrefix = "[MOCK] ";
        int total = 300;
        int totalPets = 0;
        int totalAgendamentos = 0;
        int totalVencimentos = 0;
        string[] petNames = {"Thor","Luna","Maggie","Bob","Mel","Rex","Bela","Toddy","Nina","Pipoca","Amora","Lili","Fred","Teca","Tico"};

        for (int i = 0; i < total; i++)
        {
            string nome = $"{mockPrefix}{nomes[rng.Next(nomes.Length)]} {sobrenomes[rng.Next(sobrenomes.Length)]}";
            string cpf = $"{rng.Next(100,999)}.{rng.Next(100,999)}.{rng.Next(100,999)}-{rng.Next(10,99)}";
            string telefone = $"(11) 9{rng.Next(1000,9999)}-{rng.Next(1000,9999)}";
            string rua = $"{ruas[rng.Next(ruas.Length)]}, {rng.Next(1,9999)}";
            string bairro = bairros[rng.Next(bairros.Length)];
            string cidade = cidades[rng.Next(cidades.Length)];
            string uf = ufs[rng.Next(ufs.Length)];
            string endereco = $"{rua} - {bairro}, {cidade} - {uf}";
            string cep = $"{rng.Next(10000,99999)}-{rng.Next(100,999)}";
            string email = $"cliente{i}@email.com";
            string plano = planos[rng.Next(planos.Length)];
            bool taxidog = rng.Next(2) == 0;
            bool tosa = rng.Next(2) == 0;
            decimal valor = plano == "Mensal" ? 180.00m : 110.00m;
            int banhosPorMes = plano == "Mensal" ? 4 : 2;

            for (int p = 0; p < 3; p++)
            {
                totalPets++;
                string petNome = $"{mockPrefix}{petNames[rng.Next(petNames.Length)]} {i + 1}.{p + 1}";
                string petRaca = racas[rng.Next(racas.Length)];
                string petPorte = portes[rng.Next(portes.Length)];
                string petRacao = racasPet[rng.Next(racasPet.Length)];
                string petIdade = $"{rng.Next(1,10)} anos";
                string petVermifugacao = $"{rng.Next(1,28):D2}/{(new[] {"01","02","03","04","05","06","07","08","09","10","11","12"})[rng.Next(12)]}/2025";
                string petAntipulgas = $"{rng.Next(1,28):D2}/{(new[] {"01","02","03","04","05","06","07","08","09","10","11","12"})[rng.Next(12)]}/2025";
                string petVacinacao = $"{rng.Next(1,28):D2}/{(new[] {"01","02","03","04","05","06","07","08","09","10","11","12"})[rng.Next(12)]}/2025";

                var cliente = new Cliente
                {
                    Nome = nome,
                    Telefone = telefone,
                    Endereco = endereco,
                    PetNome = petNome,
                    PetRaca = petRaca,
                    Porte = petPorte,
                    Plano = plano,
                    BanhosRestantes = plano == "Mensal" ? 4 : 2,
                    Taxidog = taxidog,
                    Tosa = tosa,
                    PetRacao = petRacao,
                    PetVermifugacao = petVermifugacao,
                    PetAntipulgas = petAntipulgas,
                    PetVacinacao = petVacinacao,
                    Valor = valor,
                    Cpf = cpf,
                    Email = email,
                    PetIdade = petIdade,
                    Cep = cep
                };
                _context.Clientes.Add(cliente);

                var pet = new Pet
                {
                    Nome = petNome,
                    Raca = petRaca,
                    Tutor = nome,
                    Porte = petPorte,
                    Racao = petRacao,
                    Vermifugacao = petVermifugacao,
                    Antipulgas = petAntipulgas,
                    Vacinacao = petVacinacao,
                    Idade = petIdade,
                    Medicamento = rng.Next(2) == 0,
                    MedicamentoNome = rng.Next(2) == 0 ? new[] {"Antialérgico","Antibiótico","Anti-inflamatório","Vermífugo","Cardíaco","Ansiolítico"}[rng.Next(6)] : null
                };
                _context.Pets.Add(pet);
            }

            if (rng.NextDouble() < 0.7)
            {
                var datasVenc = new[] {"10/06/2026","15/06/2026","10/07/2026","15/07/2026","10/08/2026","15/08/2026"};
                string vencData = datasVenc[rng.Next(datasVenc.Length)];
                string statusVenc = rng.NextDouble() < 0.6 ? "Pago" : "Pendente";
                _context.Vencimentos.Add(new Vencimento
                {
                    Cliente = nome,
                    Plano = plano,
                    VencimentoData = vencData,
                    Valor = valor,
                    Status = statusVenc
                });
                totalVencimentos++;

                if (rng.NextDouble() < 0.3)
                {
                    string vencData2 = datasVenc[rng.Next(datasVenc.Length)];
                    _context.Vencimentos.Add(new Vencimento
                    {
                        Cliente = nome,
                        Plano = plano,
                        VencimentoData = vencData2,
                        Valor = valor,
                        Status = "Pago"
                    });
                    totalVencimentos++;
                }
            }

            if (rng.NextDouble() < 0.65)
            {
                string[] petNomesParaAgenda = new string[3];
                for (int p = 0; p < 3; p++)
                    petNomesParaAgenda[p] = $"{mockPrefix}{petNames[rng.Next(petNames.Length)]} {i + 1}.{p + 1}";
                foreach (var petNomeAg in petNomesParaAgenda)
                {
                    int qtdeAgendamentos = rng.Next(1, banhosPorMes + 1);
                    for (int a = 0; a < qtdeAgendamentos; a++)
                    {
                        int dia = rng.Next(1, 29);
                        string dataAg = $"{dia:D2}/{(new[] {"06","07","08"})[rng.Next(3)]}/2026";
                        string horario = $"{rng.Next(7, 17):D2}:{new[] {"00","15","30","45"}[rng.Next(4)]}";
                        _context.Agendamentos.Add(new Agendamento
                        {
                            Data = dataAg,
                            Horario = horario,
                            Pet = petNomeAg,
                            Tutor = nome,
                            Plano = plano,
                            Status = statusAgenda[rng.Next(statusAgenda.Length)]
                        });
                        totalAgendamentos++;
                    }
                }
            }
        }

        await _context.SaveChangesAsync();
        return $"Gerados {total} clientes com {totalPets} pets, {totalAgendamentos} agendamentos e {totalVencimentos} pagamentos (dados mock).";
    }

    public async Task<int> LimparMockDataAsync()
    {
        var mockClientes = await _context.Clientes
            .Where(c => c.Nome.StartsWith("[MOCK]"))
            .ToListAsync();

        if (mockClientes.Count == 0) return 0;

        var mockPets = await _context.Pets
            .Where(p => (p.Tutor ?? "").StartsWith("[MOCK]") || (p.Nome ?? "").StartsWith("[MOCK]"))
            .ToListAsync();

        var mockAgendamentos = await _context.Agendamentos
            .Where(a => (a.Tutor ?? "").StartsWith("[MOCK]"))
            .ToListAsync();

        var mockVencimentos = await _context.Vencimentos
            .Where(v => (v.Cliente ?? "").StartsWith("[MOCK]"))
            .ToListAsync();

        _context.Agendamentos.RemoveRange(mockAgendamentos);
        _context.Vencimentos.RemoveRange(mockVencimentos);
        _context.Pets.RemoveRange(mockPets);
        _context.Clientes.RemoveRange(mockClientes);
        await _context.SaveChangesAsync();

        return mockClientes.Count;
    }
}
