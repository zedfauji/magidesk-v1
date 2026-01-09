using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionManualAdjustment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.AddColumn<string>(
                name: "PreferredLanguage",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TableTypeId",
                schema: "magidesk",
                table: "Tables",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TableSessions",
                schema: "magidesk",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TableId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PausedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TotalPausedDuration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TableTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    TotalChargeAmount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    TotalChargeCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    GuestCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ManualAdjustment = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TableTypes",
                schema: "magidesk",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    HourlyRate = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    FirstHourRate = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    MinimumMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    RoundingMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tables_TableTypeId",
                schema: "magidesk",
                table: "Tables",
                column: "TableTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TableSessions_CustomerId",
                schema: "magidesk",
                table: "TableSessions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TableSessions_StartTime",
                schema: "magidesk",
                table: "TableSessions",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_TableSessions_Status",
                schema: "magidesk",
                table: "TableSessions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TableSessions_TableId",
                schema: "magidesk",
                table: "TableSessions",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_TableSessions_TicketId",
                schema: "magidesk",
                table: "TableSessions",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TableTypes_IsActive",
                schema: "magidesk",
                table: "TableTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TableTypes_Name",
                schema: "magidesk",
                table: "TableTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_TableTypes_TableTypeId",
                schema: "magidesk",
                table: "Tables",
                column: "TableTypeId",
                principalSchema: "magidesk",
                principalTable: "TableTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_TableTypes_TableTypeId",
                schema: "magidesk",
                table: "Tables");

            migrationBuilder.DropTable(
                name: "TableSessions",
                schema: "magidesk");

            migrationBuilder.DropTable(
                name: "TableTypes",
                schema: "magidesk");

            migrationBuilder.DropIndex(
                name: "IX_Tables_TableTypeId",
                schema: "magidesk",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "PreferredLanguage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TableTypeId",
                schema: "magidesk",
                table: "Tables");
        }
    }
}
