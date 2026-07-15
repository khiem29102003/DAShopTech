using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAShopTech.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingDateToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ShippingDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingDate",
                table: "Orders");
        }
    }
}
