using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class FixedColumnProblem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CryptoTradeHistory_CurrencySymbol_CurrencySymbolId",
                table: "CryptoTradeHistory");

            migrationBuilder.DropColumn(
                name: "ValueCurrencyId",
                table: "CryptoTradeHistory");

            migrationBuilder.AlterColumn<int>(
                name: "CurrencySymbolId",
                table: "CryptoTradeHistory",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CryptoTradeHistory_CurrencySymbol_CurrencySymbolId",
                table: "CryptoTradeHistory",
                column: "CurrencySymbolId",
                principalTable: "CurrencySymbol",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CryptoTradeHistory_CurrencySymbol_CurrencySymbolId",
                table: "CryptoTradeHistory");

            migrationBuilder.AlterColumn<int>(
                name: "CurrencySymbolId",
                table: "CryptoTradeHistory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ValueCurrencyId",
                table: "CryptoTradeHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_CryptoTradeHistory_CurrencySymbol_CurrencySymbolId",
                table: "CryptoTradeHistory",
                column: "CurrencySymbolId",
                principalTable: "CurrencySymbol",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
