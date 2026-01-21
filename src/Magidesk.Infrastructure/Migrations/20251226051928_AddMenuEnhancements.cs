using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                schema: "magidesk",
                table: "MenuModifiers",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Portion",
                schema: "magidesk",
                table: "MenuModifiers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriceStrategy",
                schema: "magidesk",
                table: "MenuModifiers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ComboDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComboGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ComboDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComboGroups_ComboDefinitions_ComboDefinitionId",
                        column: x => x.ComboDefinitionId,
                        principalTable: "ComboDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComboGroupItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ComboGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    MenuItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Upcharge = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboGroupItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComboGroupItems_ComboGroups_ComboGroupId",
                        column: x => x.ComboGroupId,
                        principalTable: "ComboGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComboGroupItems_ComboGroupId",
                table: "ComboGroupItems",
                column: "ComboGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboGroupItems_MenuItemId",
                table: "ComboGroupItems",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboGroups_ComboDefinitionId",
                table: "ComboGroups",
                column: "ComboDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComboGroupItems");

            migrationBuilder.DropTable(
                name: "ComboGroups");

            migrationBuilder.DropTable(
                name: "ComboDefinitions");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                schema: "magidesk",
                table: "MenuModifiers");

            migrationBuilder.DropColumn(
                name: "Portion",
                schema: "magidesk",
                table: "MenuModifiers");

            migrationBuilder.DropColumn(
                name: "PriceStrategy",
                schema: "magidesk",
                table: "MenuModifiers");
        }
    }
}
