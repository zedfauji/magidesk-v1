using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentCategoryId",
                table: "MenuCategories",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuCategories_ParentCategoryId",
                table: "MenuCategories",
                column: "ParentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCategories_MenuCategories_ParentCategoryId",
                table: "MenuCategories",
                column: "ParentCategoryId",
                principalTable: "MenuCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCategories_MenuCategories_ParentCategoryId",
                table: "MenuCategories");

            migrationBuilder.DropIndex(
                name: "IX_MenuCategories_ParentCategoryId",
                table: "MenuCategories");

            migrationBuilder.DropColumn(
                name: "ParentCategoryId",
                table: "MenuCategories");
        }
    }
}
