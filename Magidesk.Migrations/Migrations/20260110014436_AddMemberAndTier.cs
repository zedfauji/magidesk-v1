using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberAndTier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MembershipTiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "numeric", nullable: false),
                    HourlyRateDiscount = table.Column<decimal>(type: "numeric", nullable: true),
                    IncludesFreeGuests = table.Column<bool>(type: "boolean", nullable: false),
                    FreeGuestsPerVisit = table.Column<int>(type: "integer", nullable: false),
                    MonthlyFeeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    MonthlyFeeCurrency = table.Column<string>(type: "text", nullable: false),
                    AnnualFeeAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    AnnualFeeCurrency = table.Column<string>(type: "text", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipTiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TierId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    JoinDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PrepaidBalanceAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PrepaidBalanceCurrency = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "magidesk",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Members_MembershipTiers_TierId",
                        column: x => x.TierId,
                        principalTable: "MembershipTiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_CustomerId",
                table: "Members",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_MemberNumber",
                table: "Members",
                column: "MemberNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_TierId",
                table: "Members",
                column: "TierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "MembershipTiers");
        }
    }
}
