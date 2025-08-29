using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FigureGear.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablePermissionVersion2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Permissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Permissions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "Permissions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Permissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_CreatedBy",
                table: "Permissions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_UpdatedBy",
                table: "Permissions",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Users_CreatedBy",
                table: "Permissions",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Users_UpdatedBy",
                table: "Permissions",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Users_CreatedBy",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Users_UpdatedBy",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_CreatedBy",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_UpdatedBy",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Permissions");
        }
    }
}
