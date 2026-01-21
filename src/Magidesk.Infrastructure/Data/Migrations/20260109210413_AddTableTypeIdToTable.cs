using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTableTypeIdToTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TableTypeId",
                schema: "magidesk",
                table: "Tables",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tables_TableTypeId",
                schema: "magidesk",
                table: "Tables",
                column: "TableTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_TableTypes_TableTypeId",
                schema: "magidesk",
                table: "Tables",
                column: "TableTypeId",
                principalSchema: "magidesk",
                principalTable: "TableTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_TableTypes_TableTypeId",
                schema: "magidesk",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Tables_TableTypeId",
                schema: "magidesk",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "TableTypeId",
                schema: "magidesk",
                table: "Tables");
        }
    }
}
