using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class AddTradesTablesForCrypto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CryptoTicker",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ticker = table.Column<string>(maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoTicker", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CryptoTradeHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradeTimeStamp = table.Column<DateTime>(nullable: false),
                    CryptoTickerId = table.Column<int>(nullable: false),
                    TradeValue = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoTradeHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CryptoTradeHistory_CryptoTicker_CryptoTickerId",
                        column: x => x.CryptoTickerId,
                        principalTable: "CryptoTicker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CryptoTicker_Ticker",
                table: "CryptoTicker",
                column: "Ticker",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CryptoTradeHistory_CryptoTickerId",
                table: "CryptoTradeHistory",
                column: "CryptoTickerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CryptoTradeHistory");

            migrationBuilder.DropTable(
                name: "CryptoTicker");
        }
    }
}
