using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewTradeTickerCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TickerCurrencySymbolId",
                table: "Trade",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trade_TickerCurrencySymbolId",
                table: "Trade",
                column: "TickerCurrencySymbolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trade_EnumItem_TickerCurrencySymbolId",
                table: "Trade",
                column: "TickerCurrencySymbolId",
                principalTable: "EnumItem",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trade_EnumItem_TickerCurrencySymbolId",
                table: "Trade");

            migrationBuilder.DropIndex(
                name: "IX_Trade_TickerCurrencySymbolId",
                table: "Trade");

            migrationBuilder.DropColumn(
                name: "TickerCurrencySymbolId",
                table: "Trade");
        }
    }
}
