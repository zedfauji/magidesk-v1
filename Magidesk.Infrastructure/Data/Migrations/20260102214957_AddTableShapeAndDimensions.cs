using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTableShapeAndDimensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "magidesk",
                table: "Tables",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Height",
                schema: "magidesk",
                table: "Tables",
                type: "integer",
                nullable: false,
                defaultValue: 100);

            migrationBuilder.AddColumn<Guid>(
                name: "LayoutId",
                schema: "magidesk",
                table: "Tables",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Shape",
                schema: "magidesk",
                table: "Tables",
                type: "text",
                nullable: false,
                defaultValue: "Rectangle");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "magidesk",
                table: "Tables",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Width",
                schema: "magidesk",
                table: "Tables",
                type: "integer",
                nullable: false,
                defaultValue: 100);

            migrationBuilder.CreateTable(
                name: "Floors",
                schema: "magidesk",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false, defaultValue: 2000),
                    Height = table.Column<int>(type: "integer", nullable: false, defaultValue: 2000),
                    BackgroundColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false, defaultValue: "#f8f8f8"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Floors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryAdjustments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantityDelta = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    AdjustedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryAdjustments_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TableShapes",
                schema: "magidesk",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ShapeType = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false, defaultValue: 100),
                    Height = table.Column<int>(type: "integer", nullable: false, defaultValue: 100),
                    BackgroundColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false, defaultValue: "#ffffff"),
                    BorderColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false, defaultValue: "#cccccc"),
                    BorderThickness = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    CornerRadius = table.Column<int>(type: "integer", nullable: false, defaultValue: 8),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableShapes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ContactPerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TableLayouts",
                schema: "magidesk",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FloorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Version = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableLayouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableLayouts_Floors_FloorId",
                        column: x => x.FloorId,
                        principalSchema: "magidesk",
                        principalTable: "Floors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PONumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OrderedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantityExpected = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    QuantityReceived = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsReceived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderLines_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tables_LayoutId",
                schema: "magidesk",
                table: "Tables",
                column: "LayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Floor_Name",
                schema: "magidesk",
                table: "Floors",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustments_InventoryItemId",
                table: "InventoryAdjustments",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_InventoryItemId",
                table: "PurchaseOrderLines",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderLines_PurchaseOrderId",
                table: "PurchaseOrderLines",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_VendorId",
                table: "PurchaseOrders",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_TableLayout_Name",
                schema: "magidesk",
                table: "TableLayouts",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TableLayouts_FloorId",
                schema: "magidesk",
                table: "TableLayouts",
                column: "FloorId");

            migrationBuilder.CreateIndex(
                name: "IX_TableShape_Name",
                schema: "magidesk",
                table: "TableShapes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_TableLayouts_LayoutId",
                schema: "magidesk",
                table: "Tables",
                column: "LayoutId",
                principalSchema: "magidesk",
                principalTable: "TableLayouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_TableLayouts_LayoutId",
                schema: "magidesk",
                table: "Tables");

            migrationBuilder.DropTable(
                name: "InventoryAdjustments");

            migrationBuilder.DropTable(
                name: "PurchaseOrderLines");

            migrationBuilder.DropTable(
                name: "TableLayouts",
                schema: "magidesk");

            migrationBuilder.DropTable(
                name: "TableShapes",
                schema: "magidesk");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "Floors",
                schema: "magidesk");

            migrationBuilder.DropTable(
                name: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Tables_LayoutId",
                schema: "magidesk",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "magidesk",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "Height",
                schema: "magidesk",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "LayoutId",
                schema: "magidesk",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "Shape",
                schema: "magidesk",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "magidesk",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "Width",
                schema: "magidesk",
                table: "Tables");
        }
    }
}
