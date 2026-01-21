using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddModifierGroupPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ExtraModifierPrice",
                schema: "magidesk",
                table: "ModifierGroups",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "FreeModifiers",
                schema: "magidesk",
                table: "ModifierGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraModifierPrice",
                schema: "magidesk",
                table: "ModifierGroups");

            migrationBuilder.DropColumn(
                name: "FreeModifiers",
                schema: "magidesk",
                table: "ModifierGroups");
        }
    }
}
