using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    public partial class AddTradeTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trade",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradeTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TickerId = table.Column<int>(type: "int", nullable: false),
                    TradeSize = table.Column<double>(type: "float", nullable: false),
                    TradeValue = table.Column<double>(type: "float", nullable: false),
                    TradeCurrencySymbolId = table.Column<int>(type: "int", nullable: false),
                    UserIdentityId = table.Column<int>(type: "int", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trade", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trade_EnumItem_TickerId",
                        column: x => x.TickerId,
                        principalTable: "EnumItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trade_EnumItem_TradeCurrencySymbolId",
                        column: x => x.TradeCurrencySymbolId,
                        principalTable: "EnumItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trade_UserIdentity_UserIdentityId",
                        column: x => x.UserIdentityId,
                        principalTable: "UserIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trade_TickerId",
                table: "Trade",
                column: "TickerId");

            migrationBuilder.CreateIndex(
                name: "IX_Trade_TradeCurrencySymbolId",
                table: "Trade",
                column: "TradeCurrencySymbolId");

            migrationBuilder.CreateIndex(
                name: "IX_Trade_UserIdentityId",
                table: "Trade",
                column: "UserIdentityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trade");
        }
    }
}
