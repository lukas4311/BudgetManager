using Microsoft.EntityFrameworkCore.Migrations;

namespace BudgetManager.Data.Migrations
{
    public partial class AddUserIdentityToTrades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserIdentityId",
                table: "CryptoTradeHistory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CryptoTradeHistory_UserIdentityId",
                table: "CryptoTradeHistory",
                column: "UserIdentityId");

            migrationBuilder.AddForeignKey(
                name: "FK_CryptoTradeHistory_UserIdentity_UserIdentityId",
                table: "CryptoTradeHistory",
                column: "UserIdentityId",
                principalTable: "UserIdentity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CryptoTradeHistory_UserIdentity_UserIdentityId",
                table: "CryptoTradeHistory");

            migrationBuilder.DropIndex(
                name: "IX_CryptoTradeHistory_UserIdentityId",
                table: "CryptoTradeHistory");

            migrationBuilder.DropColumn(
                name: "UserIdentityId",
                table: "CryptoTradeHistory");
        }
    }
}
