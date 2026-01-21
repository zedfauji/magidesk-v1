using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTableSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableSessions", x => x.Id);
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TableSessions",
                schema: "magidesk");
        }
    }
}
