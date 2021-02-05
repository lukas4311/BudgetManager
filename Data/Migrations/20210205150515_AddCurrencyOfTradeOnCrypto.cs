using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class AddCurrencyOfTradeOnCrypto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencySymbolId",
                table: "CryptoTradeHistory",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ValueCurrencyId",
                table: "CryptoTradeHistory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CurrencySymbol",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencySymbol", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CryptoTradeHistory_CurrencySymbolId",
                table: "CryptoTradeHistory",
                column: "CurrencySymbolId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencySymbol_Symbol",
                table: "CurrencySymbol",
                column: "Symbol",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CryptoTradeHistory_CurrencySymbol_CurrencySymbolId",
                table: "CryptoTradeHistory",
                column: "CurrencySymbolId",
                principalTable: "CurrencySymbol",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CryptoTradeHistory_CurrencySymbol_CurrencySymbolId",
                table: "CryptoTradeHistory");

            migrationBuilder.DropTable(
                name: "CurrencySymbol");

            migrationBuilder.DropIndex(
                name: "IX_CryptoTradeHistory_CurrencySymbolId",
                table: "CryptoTradeHistory");

            migrationBuilder.DropColumn(
                name: "CurrencySymbolId",
                table: "CryptoTradeHistory");

            migrationBuilder.DropColumn(
                name: "ValueCurrencyId",
                table: "CryptoTradeHistory");
        }
    }
}
