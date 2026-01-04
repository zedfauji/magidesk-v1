using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddPrinterDetailedConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Dpi",
                table: "PrinterMappings",
                type: "integer",
                nullable: false,
                defaultValue: 203);

            migrationBuilder.AddColumn<int>(
                name: "PaperWidthMm",
                table: "PrinterMappings",
                type: "integer",
                nullable: false,
                defaultValue: 80);

            migrationBuilder.AddColumn<int>(
                name: "PrintableWidthChars",
                table: "PrinterMappings",
                type: "integer",
                nullable: false,
                defaultValue: 48);

            migrationBuilder.AddColumn<bool>(
                name: "SupportsCashDrawer",
                table: "PrinterMappings",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "SupportsImages",
                table: "PrinterMappings",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "SupportsQr",
                table: "PrinterMappings",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowReprint",
                table: "PrinterGroups",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "CutBehavior",
                table: "PrinterGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "FallbackPrinterGroupId",
                table: "PrinterGroups",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "PrinterGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RetryDelayMs",
                table: "PrinterGroups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ShowPrices",
                table: "PrinterGroups",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dpi",
                table: "PrinterMappings");

            migrationBuilder.DropColumn(
                name: "PaperWidthMm",
                table: "PrinterMappings");

            migrationBuilder.DropColumn(
                name: "PrintableWidthChars",
                table: "PrinterMappings");

            migrationBuilder.DropColumn(
                name: "SupportsCashDrawer",
                table: "PrinterMappings");

            migrationBuilder.DropColumn(
                name: "SupportsImages",
                table: "PrinterMappings");

            migrationBuilder.DropColumn(
                name: "SupportsQr",
                table: "PrinterMappings");

            migrationBuilder.DropColumn(
                name: "AllowReprint",
                table: "PrinterGroups");

            migrationBuilder.DropColumn(
                name: "CutBehavior",
                table: "PrinterGroups");

            migrationBuilder.DropColumn(
                name: "FallbackPrinterGroupId",
                table: "PrinterGroups");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "PrinterGroups");

            migrationBuilder.DropColumn(
                name: "RetryDelayMs",
                table: "PrinterGroups");

            migrationBuilder.DropColumn(
                name: "ShowPrices",
                table: "PrinterGroups");
        }
    }
}
