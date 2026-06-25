using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SantoExpedito.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Agendamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Horario = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Pet = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Tutor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Plano = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendamentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Endereco = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PetNome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PetRaca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Porte = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Plano = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BanhosRestantes = table.Column<int>(type: "int", nullable: false),
                    Taxidog = table.Column<bool>(type: "bit", nullable: false),
                    Tosa = table.Column<bool>(type: "bit", nullable: false),
                    PetRacao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PetVermifugacao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PetAntipulgas = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PetVacinacao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DiasBanho = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatasBanho = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PetIdade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Cpf = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Raca = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Tutor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Porte = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Racao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Vermifugacao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Antipulgas = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Vacinacao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Idade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vencimentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cliente = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Plano = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    VencimentoData = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vencimentos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminUsers");

            migrationBuilder.DropTable(
                name: "Agendamentos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Pets");

            migrationBuilder.DropTable(
                name: "Vencimentos");
        }
    }
}
