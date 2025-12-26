using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKitchenOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "magidesk");

            migrationBuilder.AlterColumn<string>(
                name: "TotalCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "TaxCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "SubtotalCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceChargeCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "PaidCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "DueCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "DiscountCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryChargeCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "AdvanceCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "AdjustmentCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AddColumn<bool>(
                name: "PriceIncludesTax",
                table: "Tickets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "TipsExceedCurrency",
                table: "Payments",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "TipsCurrency",
                table: "Payments",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "TenderCurrency",
                table: "Payments",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "ChangeCurrency",
                table: "Payments",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "AmountCurrency",
                table: "Payments",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "UnitPriceCurrency",
                table: "OrderLines",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "TotalWithoutModifiersCurrency",
                table: "OrderLines",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "TotalCurrency",
                table: "OrderLines",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "TaxWithoutModifiersCurrency",
                table: "OrderLines",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "TaxCurrency",
                table: "OrderLines",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "SubtotalWithoutModifiersCurrency",
                table: "OrderLines",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "SubtotalCurrency",
                table: "OrderLines",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "DiscountCurrency",
                table: "OrderLines",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.CreateTable(
                name: "KitchenOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TableNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuModifiers",
                schema: "magidesk",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ModifierGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifierType = table.Column<int>(type: "integer", nullable: false),
                    BasePrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    BasePriceCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false, defaultValue: 0m),
                    ShouldPrintToKitchen = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsSectionWisePrice = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    SectionName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MultiplierName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuModifiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModifierGroups",
                schema: "magidesk",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    MinSelections = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    MaxSelections = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    AllowMultipleSelections = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModifierGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderTypes",
                schema: "magidesk",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CloseOnPaid = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AllowSeatBasedOrder = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AllowToAddTipsLater = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsBarTab = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Properties = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartTime = table.Column<long>(type: "bigint", nullable: false),
                    EndTime = table.Column<long>(type: "bigint", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                schema: "magidesk",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TableNumber = table.Column<int>(type: "integer", nullable: false),
                    FloorId = table.Column<Guid>(type: "uuid", nullable: true),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Y = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Available"),
                    CurrentTicketId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KitchenOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KitchenOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    DestinationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Modifiers = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchenOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KitchenOrderItems_KitchenOrders_KitchenOrderId",
                        column: x => x.KitchenOrderId,
                        principalTable: "KitchenOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KitchenOrderItems_KitchenOrderId",
                table: "KitchenOrderItems",
                column: "KitchenOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuModifiers_DisplayOrder",
                schema: "magidesk",
                table: "MenuModifiers",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_MenuModifiers_IsActive",
                schema: "magidesk",
                table: "MenuModifiers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MenuModifiers_ModifierGroupId",
                schema: "magidesk",
                table: "MenuModifiers",
                column: "ModifierGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuModifiers_Name",
                schema: "magidesk",
                table: "MenuModifiers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ModifierGroups_DisplayOrder",
                schema: "magidesk",
                table: "ModifierGroups",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ModifierGroups_IsActive",
                schema: "magidesk",
                table: "ModifierGroups",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ModifierGroups_Name",
                schema: "magidesk",
                table: "ModifierGroups",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderTypes_IsActive",
                schema: "magidesk",
                table: "OrderTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTypes_Name",
                schema: "magidesk",
                table: "OrderTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_IsActive",
                table: "Shifts",
                column: "IsActive",
                filter: "\"IsActive\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_CurrentTicketId",
                schema: "magidesk",
                table: "Tables",
                column: "CurrentTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_FloorId",
                schema: "magidesk",
                table: "Tables",
                column: "FloorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_IsActive",
                schema: "magidesk",
                table: "Tables",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_Status",
                schema: "magidesk",
                table: "Tables",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_TableNumber",
                schema: "magidesk",
                table: "Tables",
                column: "TableNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KitchenOrderItems");

            migrationBuilder.DropTable(
                name: "MenuModifiers",
                schema: "magidesk");

            migrationBuilder.DropTable(
                name: "ModifierGroups",
                schema: "magidesk");

            migrationBuilder.DropTable(
                name: "OrderTypes",
                schema: "magidesk");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "Tables",
                schema: "magidesk");

            migrationBuilder.DropTable(
                name: "KitchenOrders");

            migrationBuilder.DropColumn(
                name: "PriceIncludesTax",
                table: "Tickets");

            migrationBuilder.AlterColumn<string>(
                name: "TotalCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "TaxCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "SubtotalCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceChargeCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "PaidCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "DueCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "DiscountCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryChargeCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "AdvanceCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "AdjustmentCurrency",
                table: "Tickets",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "TipsExceedCurrency",
                table: "Payments",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "TipsCurrency",
                table: "Payments",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "TenderCurrency",
                table: "Payments",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "ChangeCurrency",
                table: "Payments",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "AmountCurrency",
                table: "Payments",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "UnitPriceCurrency",
                table: "OrderLines",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "TotalWithoutModifiersCurrency",
                table: "OrderLines",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "TotalCurrency",
                table: "OrderLines",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "TaxWithoutModifiersCurrency",
                table: "OrderLines",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "TaxCurrency",
                table: "OrderLines",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "SubtotalWithoutModifiersCurrency",
                table: "OrderLines",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "SubtotalCurrency",
                table: "OrderLines",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");

            migrationBuilder.AlterColumn<string>(
                name: "DiscountCurrency",
                table: "OrderLines",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "USD");
        }
    }
}
