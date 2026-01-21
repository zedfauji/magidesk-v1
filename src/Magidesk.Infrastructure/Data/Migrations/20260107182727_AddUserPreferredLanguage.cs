using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPreferredLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreferredLanguage",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<double>(
                name: "Y",
                schema: "magidesk",
                table: "Tables",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "X",
                schema: "magidesk",
                table: "Tables",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "Width",
                schema: "magidesk",
                table: "Tables",
                type: "double precision",
                nullable: false,
                defaultValue: 100.0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 100);

            migrationBuilder.AlterColumn<double>(
                name: "Height",
                schema: "magidesk",
                table: "Tables",
                type: "double precision",
                nullable: false,
                defaultValue: 100.0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 100);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_MenuCategories_CategoryId",
                table: "MenuItems");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItems_MenuGroups_GroupId",
                table: "MenuItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PrinterGroups_PrintTemplates_KitchenTemplateId",
                table: "PrinterGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_PrinterGroups_PrintTemplates_ReceiptTemplateId",
                table: "PrinterGroups");

            migrationBuilder.DropTable(
                name: "PrintTemplates");

            migrationBuilder.DropIndex(
                name: "IX_PrinterGroups_KitchenTemplateId",
                table: "PrinterGroups");

            migrationBuilder.DropIndex(
                name: "IX_PrinterGroups_ReceiptTemplateId",
                table: "PrinterGroups");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_CategoryId",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_GroupId",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "PreferredLanguage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDraft",
                schema: "magidesk",
                table: "TableLayouts");

            migrationBuilder.DropColumn(
                name: "CutEnabled",
                table: "PrinterMappings");

            migrationBuilder.DropColumn(
                name: "Dpi",
                table: "PrinterMappings");

            migrationBuilder.DropColumn(
                name: "Format",
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
                name: "KitchenTemplateId",
                table: "PrinterGroups");

            migrationBuilder.DropColumn(
                name: "ReceiptTemplateId",
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

            migrationBuilder.DropColumn(
                name: "PrinterGroupId",
                table: "MenuGroups");

            migrationBuilder.DropColumn(
                name: "PrinterGroupId",
                table: "MenuCategories");

            migrationBuilder.AlterColumn<int>(
                name: "Y",
                schema: "magidesk",
                table: "Tables",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldDefaultValue: 0.0);

            migrationBuilder.AlterColumn<int>(
                name: "X",
                schema: "magidesk",
                table: "Tables",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldDefaultValue: 0.0);

            migrationBuilder.AlterColumn<int>(
                name: "Width",
                schema: "magidesk",
                table: "Tables",
                type: "integer",
                nullable: false,
                defaultValue: 100,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldDefaultValue: 100.0);

            migrationBuilder.AlterColumn<int>(
                name: "Height",
                schema: "magidesk",
                table: "Tables",
                type: "integer",
                nullable: false,
                defaultValue: 100,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldDefaultValue: 100.0);
        }
    }
}
