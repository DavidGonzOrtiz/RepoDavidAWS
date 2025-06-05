using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiAppMvc.Migrations
{
    /// <inheritdoc />
    public partial class M4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categories",
                table: "Empresas");

            migrationBuilder.AddColumn<int>(
                name: "Categories",
                table: "Eventos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categories",
                table: "Eventos");

            migrationBuilder.AddColumn<int>(
                name: "Categories",
                table: "Empresas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
