using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCardConfigParityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowManualEntry",
                table: "MerchantGatewayConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowTipAdjustment",
                table: "MerchantGatewayConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "CardTypesAccepted",
                table: "MerchantGatewayConfigurations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "VISA,MC,AMEX,DISC");

            migrationBuilder.AddColumn<bool>(
                name: "EnablePreAuth",
                table: "MerchantGatewayConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsExternalTerminal",
                table: "MerchantGatewayConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "SignatureThreshold",
                table: "MerchantGatewayConfigurations",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 25m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowManualEntry",
                table: "MerchantGatewayConfigurations");

            migrationBuilder.DropColumn(
                name: "AllowTipAdjustment",
                table: "MerchantGatewayConfigurations");

            migrationBuilder.DropColumn(
                name: "CardTypesAccepted",
                table: "MerchantGatewayConfigurations");

            migrationBuilder.DropColumn(
                name: "EnablePreAuth",
                table: "MerchantGatewayConfigurations");

            migrationBuilder.DropColumn(
                name: "IsExternalTerminal",
                table: "MerchantGatewayConfigurations");

            migrationBuilder.DropColumn(
                name: "SignatureThreshold",
                table: "MerchantGatewayConfigurations");
        }
    }
}
