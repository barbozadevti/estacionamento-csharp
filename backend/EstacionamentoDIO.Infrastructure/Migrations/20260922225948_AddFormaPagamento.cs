using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstacionamentoDIO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFormaPagamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormaPagamento",
                table: "Veiculos",
                type: "TEXT",
                maxLength: 30,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormaPagamento",
                table: "Veiculos");
        }
    }
}
