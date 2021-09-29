using Microsoft.EntityFrameworkCore.Migrations;

namespace BudgetManager.Data.Migrations
{
    public partial class ChangedRelationOnBankAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccount_UserData_UserDataId",
                table: "BankAccount");

            migrationBuilder.DropIndex(
                name: "IX_BankAccount_UserDataId",
                table: "BankAccount");

            migrationBuilder.DropColumn(
                name: "UserDataId",
                table: "BankAccount");

            migrationBuilder.AddColumn<int>(
                name: "UserIdentityId",
                table: "BankAccount",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_UserIdentityId",
                table: "BankAccount",
                column: "UserIdentityId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccount_UserIdentity_UserIdentityId",
                table: "BankAccount",
                column: "UserIdentityId",
                principalTable: "UserIdentity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccount_UserIdentity_UserIdentityId",
                table: "BankAccount");

            migrationBuilder.DropIndex(
                name: "IX_BankAccount_UserIdentityId",
                table: "BankAccount");

            migrationBuilder.DropColumn(
                name: "UserIdentityId",
                table: "BankAccount");

            migrationBuilder.AddColumn<int>(
                name: "UserDataId",
                table: "BankAccount",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_UserDataId",
                table: "BankAccount",
                column: "UserDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccount_UserData_UserDataId",
                table: "BankAccount",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
