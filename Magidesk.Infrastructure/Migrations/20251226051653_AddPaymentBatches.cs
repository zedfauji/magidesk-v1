using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentBatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupSettlements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MasterPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildTicketIds = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    Strategy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupSettlements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerminalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    OpenedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GatewayBatchId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentBatches", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupSettlements_MasterPaymentId",
                table: "GroupSettlements",
                column: "MasterPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentBatches_Status",
                table: "PaymentBatches",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentBatches_TerminalId",
                table: "PaymentBatches",
                column: "TerminalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupSettlements");

            migrationBuilder.DropTable(
                name: "PaymentBatches");
        }
    }
}
