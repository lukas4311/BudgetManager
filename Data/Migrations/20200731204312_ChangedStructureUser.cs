using Microsoft.EntityFrameworkCore.Migrations;

namespace BudgetManager.Data.Migrations
{
    public partial class ChangedStructureUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserIdentity_UserData_UserDataId",
                table: "UserIdentity");

            migrationBuilder.DropIndex(
                name: "IX_UserIdentity_UserDataId",
                table: "UserIdentity");

            migrationBuilder.DropColumn(
                name: "UserDataId",
                table: "UserIdentity");

            migrationBuilder.CreateIndex(
                name: "IX_UserData_UserIdentityId",
                table: "UserData",
                column: "UserIdentityId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserData_UserIdentity_UserIdentityId",
                table: "UserData",
                column: "UserIdentityId",
                principalTable: "UserIdentity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserData_UserIdentity_UserIdentityId",
                table: "UserData");

            migrationBuilder.DropIndex(
                name: "IX_UserData_UserIdentityId",
                table: "UserData");

            migrationBuilder.AddColumn<int>(
                name: "UserDataId",
                table: "UserIdentity",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentity_UserDataId",
                table: "UserIdentity",
                column: "UserDataId",
                unique: true,
                filter: "[UserDataId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_UserIdentity_UserData_UserDataId",
                table: "UserIdentity",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
