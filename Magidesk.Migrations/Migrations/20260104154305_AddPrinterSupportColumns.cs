using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Magidesk.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddPrinterSupportColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create PrinterGroups
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

            // 2. Create PrinterMappings
            migrationBuilder.CreateTable(
                name: "PrinterMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerminalId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrinterGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhysicalPrinterName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Format = table.Column<int>(type: "integer", nullable: false),
                    CutEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrinterMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrinterMappings_PrinterGroups_PrinterGroupId",
                        column: x => x.PrinterGroupId,
                        principalTable: "PrinterGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Index for PrinterMappings FK
            migrationBuilder.CreateIndex(
                name: "IX_PrinterMappings_PrinterGroupId",
                table: "PrinterMappings",
                column: "PrinterGroupId");

            // 3. Add PrinterGroupId to MenuItems
            migrationBuilder.AddColumn<Guid>(
                name: "PrinterGroupId",
                table: "MenuItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_PrinterGroupId",
                table: "MenuItems",
                column: "PrinterGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItems_PrinterGroups_PrinterGroupId",
                table: "MenuItems",
                column: "PrinterGroupId",
                principalTable: "PrinterGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_PrinterGroups_PrinterGroupId",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_PrinterGroupId",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "PrinterGroupId",
                table: "MenuItems");

            migrationBuilder.DropTable(
                name: "PrinterMappings");

            migrationBuilder.DropTable(
                name: "PrinterGroups");
        }
    }
}
