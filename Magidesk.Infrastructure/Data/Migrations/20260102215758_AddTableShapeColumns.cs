using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Magidesk.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTableShapeColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServerSectionId",
                schema: "magidesk",
                table: "Tables",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServerSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ServerId = table.Column<Guid>(type: "uuid", nullable: false),
                    TableIds = table.Column<List<Guid>>(type: "uuid[]", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerSections_Users_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tables_ServerSectionId",
                schema: "magidesk",
                table: "Tables",
                column: "ServerSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerSections_ServerId",
                table: "ServerSections",
                column: "ServerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_ServerSections_ServerSectionId",
                schema: "magidesk",
                table: "Tables",
                column: "ServerSectionId",
                principalTable: "ServerSections",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_ServerSections_ServerSectionId",
                schema: "magidesk",
                table: "Tables");

            migrationBuilder.DropTable(
                name: "ServerSections");

            migrationBuilder.DropIndex(
                name: "IX_Tables_ServerSectionId",
                schema: "magidesk",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "ServerSectionId",
                schema: "magidesk",
                table: "Tables");
        }
    }
}
