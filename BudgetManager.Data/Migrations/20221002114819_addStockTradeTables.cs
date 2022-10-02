using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    public partial class addStockTradeTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockTicker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ticker = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTicker", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockTradeHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradeTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StockTickerId = table.Column<int>(type: "int", nullable: false),
                    TradeSize = table.Column<double>(type: "float", nullable: false),
                    TradeValue = table.Column<double>(type: "float", nullable: false),
                    CurrencySymbolId = table.Column<int>(type: "int", nullable: false),
                    UserIdentityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTradeHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTradeHistory_CurrencySymbol_CurrencySymbolId",
                        column: x => x.CurrencySymbolId,
                        principalTable: "CurrencySymbol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockTradeHistory_StockTicker_StockTickerId",
                        column: x => x.StockTickerId,
                        principalTable: "StockTicker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockTradeHistory_UserIdentity_UserIdentityId",
                        column: x => x.UserIdentityId,
                        principalTable: "UserIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockTicker_Ticker",
                table: "StockTicker",
                column: "Ticker",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTradeHistory_CurrencySymbolId",
                table: "StockTradeHistory",
                column: "CurrencySymbolId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTradeHistory_StockTickerId",
                table: "StockTradeHistory",
                column: "StockTickerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTradeHistory_UserIdentityId",
                table: "StockTradeHistory",
                column: "UserIdentityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockTradeHistory");

            migrationBuilder.DropTable(
                name: "StockTicker");
        }
    }
}
