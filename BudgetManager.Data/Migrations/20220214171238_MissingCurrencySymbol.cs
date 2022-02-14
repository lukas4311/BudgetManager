using Microsoft.EntityFrameworkCore.Migrations;

namespace BudgetManager.Data.Migrations
{
    public partial class MissingCurrencySymbol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrencySymbolId",
                table: "OtherInvestment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OtherInvestment_CurrencySymbolId",
                table: "OtherInvestment",
                column: "CurrencySymbolId");

            migrationBuilder.AddForeignKey(
                name: "FK_OtherInvestment_CurrencySymbol_CurrencySymbolId",
                table: "OtherInvestment",
                column: "CurrencySymbolId",
                principalTable: "CurrencySymbol",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OtherInvestment_CurrencySymbol_CurrencySymbolId",
                table: "OtherInvestment");

            migrationBuilder.DropIndex(
                name: "IX_OtherInvestment_CurrencySymbolId",
                table: "OtherInvestment");

            migrationBuilder.DropColumn(
                name: "CurrencySymbolId",
                table: "OtherInvestment");
        }
    }
}
