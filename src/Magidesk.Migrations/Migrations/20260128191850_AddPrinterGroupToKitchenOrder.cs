using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddPrinterGroupToKitchenOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PrinterGroupId",
                table: "KitchenOrders",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrinterGroupId",
                table: "KitchenOrders");
        }
    }
}
