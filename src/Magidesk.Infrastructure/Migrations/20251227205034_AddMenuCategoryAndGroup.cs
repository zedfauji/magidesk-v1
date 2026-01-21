using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuCategoryAndGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLineModifiers_OrderLines_OrderLineId2",
                table: "OrderLineModifiers");

            migrationBuilder.DropIndex(
                name: "IX_OrderLineModifiers_OrderLineId2",
                table: "OrderLineModifiers");

            migrationBuilder.RenameColumn(
                name: "OrderLineId2",
                table: "OrderLineModifiers",
                newName: "ParentOrderLineModifierId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DispatchedTime",
                table: "Tickets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadyTime",
                table: "Tickets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BatchId",
                table: "Payments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisibleInLoginScreen",
                schema: "magidesk",
                table: "OrderTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "ModifierId",
                table: "OrderLineModifiers",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "OrderLineModifiers",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BasePriceCurrency",
                table: "OrderLineModifiers",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<Guid>(
                name: "ModifierGroupId",
                table: "OrderLineModifiers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderLineId1",
                table: "OrderLineModifiers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PortionValue",
                table: "OrderLineModifiers",
                type: "numeric(5,4)",
                precision: 5,
                scale: 4,
                nullable: false,
                defaultValue: 1.0m);

            migrationBuilder.AddColumn<int>(
                name: "PriceStrategy",
                table: "OrderLineModifiers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "Discounts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MenuCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsVisible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsBeverage = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ButtonColor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Barcode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PriceCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    TaxRate = table.Column<decimal>(type: "numeric(5,4)", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    ComboDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsVisible = table.Column<bool>(type: "boolean", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    ShowInKiosk = table.Column<bool>(type: "boolean", nullable: false),
                    IsStockItem = table.Column<bool>(type: "boolean", nullable: false),
                    ShouldPrintToKitchen = table.Column<bool>(type: "boolean", nullable: false),
                    PrinterGroupId = table.Column<Guid>(type: "uuid", nullable: true),
                    Properties = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MerchantGatewayConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerminalId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MerchantId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EncryptedApiKey = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    GatewayUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantGatewayConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Permissions = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EncryptedPin = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EncryptedPassword = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    HourlyRateCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsVisible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ButtonColor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuGroups_MenuCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "MenuCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItemModifierGroups",
                columns: table => new
                {
                    MenuItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModifierGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItemModifierGroups", x => new { x.MenuItemId, x.ModifierGroupId });
                    table.ForeignKey(
                        name: "FK_MenuItemModifierGroups_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuItemModifierGroups_ModifierGroups_ModifierGroupId",
                        column: x => x.ModifierGroupId,
                        principalSchema: "magidesk",
                        principalTable: "ModifierGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BatchId",
                table: "Payments",
                column: "BatchId",
                filter: "\"BatchId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLineModifiers_OrderLineId1",
                table: "OrderLineModifiers",
                column: "OrderLineId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderLineModifiers_ParentOrderLineModifierId",
                table: "OrderLineModifiers",
                column: "ParentOrderLineModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_CouponCode",
                table: "Discounts",
                column: "CouponCode",
                unique: true,
                filter: "\"CouponCode\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCategories_Name",
                table: "MenuCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuGroups_CategoryId",
                table: "MenuGroups",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuGroups_CategoryId_Name",
                table: "MenuGroups",
                columns: new[] { "CategoryId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemModifierGroups_ModifierGroupId",
                table: "MenuItemModifierGroups",
                column: "ModifierGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantGatewayConfigurations_TerminalId",
                table: "MerchantGatewayConfigurations",
                column: "TerminalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuModifiers_ModifierGroups_ModifierGroupId",
                schema: "magidesk",
                table: "MenuModifiers",
                column: "ModifierGroupId",
                principalSchema: "magidesk",
                principalTable: "ModifierGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLineModifiers_OrderLineModifiers_ParentOrderLineModifi~",
                table: "OrderLineModifiers",
                column: "ParentOrderLineModifierId",
                principalTable: "OrderLineModifiers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLineModifiers_OrderLines_OrderLineId1",
                table: "OrderLineModifiers",
                column: "OrderLineId1",
                principalTable: "OrderLines",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuModifiers_ModifierGroups_ModifierGroupId",
                schema: "magidesk",
                table: "MenuModifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLineModifiers_OrderLineModifiers_ParentOrderLineModifi~",
                table: "OrderLineModifiers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLineModifiers_OrderLines_OrderLineId1",
                table: "OrderLineModifiers");

            migrationBuilder.DropTable(
                name: "MenuGroups");

            migrationBuilder.DropTable(
                name: "MenuItemModifierGroups");

            migrationBuilder.DropTable(
                name: "MerchantGatewayConfigurations");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "MenuCategories");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_Payments_BatchId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_OrderLineModifiers_OrderLineId1",
                table: "OrderLineModifiers");

            migrationBuilder.DropIndex(
                name: "IX_OrderLineModifiers_ParentOrderLineModifierId",
                table: "OrderLineModifiers");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_CouponCode",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "DispatchedTime",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ReadyTime",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsVisibleInLoginScreen",
                schema: "magidesk",
                table: "OrderTypes");

            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "OrderLineModifiers");

            migrationBuilder.DropColumn(
                name: "BasePriceCurrency",
                table: "OrderLineModifiers");

            migrationBuilder.DropColumn(
                name: "ModifierGroupId",
                table: "OrderLineModifiers");

            migrationBuilder.DropColumn(
                name: "OrderLineId1",
                table: "OrderLineModifiers");

            migrationBuilder.DropColumn(
                name: "PortionValue",
                table: "OrderLineModifiers");

            migrationBuilder.DropColumn(
                name: "PriceStrategy",
                table: "OrderLineModifiers");

            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Discounts");

            migrationBuilder.RenameColumn(
                name: "ParentOrderLineModifierId",
                table: "OrderLineModifiers",
                newName: "OrderLineId2");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModifierId",
                table: "OrderLineModifiers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderLineModifiers_OrderLineId2",
                table: "OrderLineModifiers",
                column: "OrderLineId2",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLineModifiers_OrderLines_OrderLineId2",
                table: "OrderLineModifiers",
                column: "OrderLineId2",
                principalTable: "OrderLines",
                principalColumn: "Id");
        }
    }
}
