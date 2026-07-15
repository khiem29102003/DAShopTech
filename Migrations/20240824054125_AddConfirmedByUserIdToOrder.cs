using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAShopTech.Migrations
{
    /// <inheritdoc />
    public partial class AddConfirmedByUserIdToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConfirmedByUserId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ConfirmedByUserId",
                table: "Orders",
                column: "ConfirmedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_ConfirmedByUserId",
                table: "Orders",
                column: "ConfirmedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_ConfirmedByUserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ConfirmedByUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ConfirmedByUserId",
                table: "Orders");
        }
    }
}
