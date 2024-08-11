using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovingOldTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CryptoTradeHistory");

            migrationBuilder.DropTable(
                name: "StockTradeHistory");

            migrationBuilder.DropTable(
                name: "CryptoTicker");

            migrationBuilder.DropTable(
                name: "StockTicker");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CryptoTicker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ticker = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoTicker", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockTicker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ticker = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTicker", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CryptoTradeHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CryptoTickerId = table.Column<int>(type: "int", nullable: false),
                    CurrencySymbolId = table.Column<int>(type: "int", nullable: false),
                    UserIdentityId = table.Column<int>(type: "int", nullable: false),
                    TradeSize = table.Column<double>(type: "float", nullable: false),
                    TradeTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TradeValue = table.Column<double>(type: "float", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_CryptoTradeHistory_CurrencySymbol_CurrencySymbolId",
                        column: x => x.CurrencySymbolId,
                        principalTable: "CurrencySymbol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CryptoTradeHistory_UserIdentity_UserIdentityId",
                        column: x => x.UserIdentityId,
                        principalTable: "UserIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockTradeHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencySymbolId = table.Column<int>(type: "int", nullable: false),
                    StockTickerId = table.Column<int>(type: "int", nullable: false),
                    UserIdentityId = table.Column<int>(type: "int", nullable: false),
                    TradeSize = table.Column<double>(type: "float", nullable: false),
                    TradeTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TradeValue = table.Column<double>(type: "float", nullable: false)
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
                name: "IX_CryptoTicker_Ticker",
                table: "CryptoTicker",
                column: "Ticker",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CryptoTradeHistory_CryptoTickerId",
                table: "CryptoTradeHistory",
                column: "CryptoTickerId");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoTradeHistory_CurrencySymbolId",
                table: "CryptoTradeHistory",
                column: "CurrencySymbolId");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoTradeHistory_UserIdentityId",
                table: "CryptoTradeHistory",
                column: "UserIdentityId");

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
    }
}
