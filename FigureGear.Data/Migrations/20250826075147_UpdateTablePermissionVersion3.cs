using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FigureGear.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablePermissionVersion3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Permissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Permissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_DeletedBy",
                table: "Permissions",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Key",
                table: "Permissions",
                column: "Key",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Users_DeletedBy",
                table: "Permissions",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Users_DeletedBy",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_DeletedBy",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_Key",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Permissions");
        }
    }
}
