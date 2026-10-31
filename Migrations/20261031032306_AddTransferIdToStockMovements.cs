using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySaaS.Migrations
{
    /// <inheritdoc />
    public partial class AddTransferIdToStockMovements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TransferId",
                table: "StockMovements",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransferId",
                table: "StockMovements");
        }
    }
}
