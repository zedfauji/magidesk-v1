using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPrinterConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrinterGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrinterGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrinterMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerminalId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrinterGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhysicalPrinterName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrinterMappings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrinterMappings_TerminalId_PrinterGroupId",
                table: "PrinterMappings",
                columns: new[] { "TerminalId", "PrinterGroupId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrinterGroups");

            migrationBuilder.DropTable(
                name: "PrinterMappings");
        }
    }
}
