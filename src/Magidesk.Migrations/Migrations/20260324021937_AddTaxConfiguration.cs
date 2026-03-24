using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "magidesk",
                table: "OrderTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DefaultTerminalRole",
                schema: "magidesk",
                table: "OrderTypes",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TaxConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(5,4)", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Country = table.Column<string>(type: "varchar(2)", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxConfigurations", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TaxConfigurations",
                columns: new[] { "Id", "Rate", "EffectiveFrom", "Country", "Version" },
                values: new object[] {
                    new Guid("b1a2c3d4-e5f6-7890-abcd-ef1234567890"),
                    0.16m,
                    new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    "MX",
                    1
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaxConfigurations");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "magidesk",
                table: "OrderTypes");

            migrationBuilder.DropColumn(
                name: "DefaultTerminalRole",
                schema: "magidesk",
                table: "OrderTypes");
        }
    }
}
