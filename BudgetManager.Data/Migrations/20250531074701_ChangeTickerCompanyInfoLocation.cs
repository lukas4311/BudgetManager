using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTickerCompanyInfoLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trade_EnumItem_TickerCurrencySymbolId",
                table: "Trade");

            migrationBuilder.AddColumn<int>(
                name: "TickerAdjustedInfoId",
                table: "Trade",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TickerAdjustedInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceTicker = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CompanyInfoTicker = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TickerAdjustedInfo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trade_TickerAdjustedInfoId",
                table: "Trade",
                column: "TickerAdjustedInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trade_EnumItem_TickerCurrencySymbolId",
                table: "Trade",
                column: "TickerCurrencySymbolId",
                principalTable: "EnumItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trade_TickerAdjustedInfo_TickerAdjustedInfoId",
                table: "Trade",
                column: "TickerAdjustedInfoId",
                principalTable: "TickerAdjustedInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trade_EnumItem_TickerCurrencySymbolId",
                table: "Trade");

            migrationBuilder.DropForeignKey(
                name: "FK_Trade_TickerAdjustedInfo_TickerAdjustedInfoId",
                table: "Trade");

            migrationBuilder.DropTable(
                name: "TickerAdjustedInfo");

            migrationBuilder.DropIndex(
                name: "IX_Trade_TickerAdjustedInfoId",
                table: "Trade");

            migrationBuilder.DropColumn(
                name: "TickerAdjustedInfoId",
                table: "Trade");

            migrationBuilder.AddForeignKey(
                name: "FK_Trade_EnumItem_TickerCurrencySymbolId",
                table: "Trade",
                column: "TickerCurrencySymbolId",
                principalTable: "EnumItem",
                principalColumn: "Id");
        }
    }
}
