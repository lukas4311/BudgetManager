using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class StockSplitChangeTickerReferenceToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockSplit_StockTicker_StockTickerId",
                table: "StockSplit");

            migrationBuilder.RenameColumn(
                name: "StockTickerId",
                table: "StockSplit",
                newName: "TickerId");

            migrationBuilder.RenameIndex(
                name: "IX_StockSplit_StockTickerId",
                table: "StockSplit",
                newName: "IX_StockSplit_TickerId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockSplit_EnumItem_TickerId",
                table: "StockSplit",
                column: "TickerId",
                principalTable: "EnumItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockSplit_EnumItem_TickerId",
                table: "StockSplit");

            migrationBuilder.RenameColumn(
                name: "TickerId",
                table: "StockSplit",
                newName: "StockTickerId");

            migrationBuilder.RenameIndex(
                name: "IX_StockSplit_TickerId",
                table: "StockSplit",
                newName: "IX_StockSplit_StockTickerId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockSplit_StockTicker_StockTickerId",
                table: "StockSplit",
                column: "StockTickerId",
                principalTable: "StockTicker",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
