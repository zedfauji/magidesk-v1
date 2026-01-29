using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddKitchenOrderLifecycleTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAt",
                table: "OrderLines",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentToKitchenAt",
                table: "OrderLines",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveredAt",
                table: "KitchenOrders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentToKitchenAt",
                table: "KitchenOrders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveredAt",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "SentToKitchenAt",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "DeliveredAt",
                table: "KitchenOrders");

            migrationBuilder.DropColumn(
                name: "SentToKitchenAt",
                table: "KitchenOrders");
        }
    }
}
