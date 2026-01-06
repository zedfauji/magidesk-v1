using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDraftCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                schema: "magidesk",
                table: "TableLayouts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDraft",
                schema: "magidesk",
                table: "TableLayouts");

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
