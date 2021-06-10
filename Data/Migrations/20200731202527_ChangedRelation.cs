using Microsoft.EntityFrameworkCore.Migrations;

namespace BudgetManager.Data.Migrations
{
    public partial class ChangedRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserIdentity_UserData_UserDataId",
                table: "UserIdentity");

            migrationBuilder.DropIndex(
                name: "IX_UserIdentity_UserDataId",
                table: "UserIdentity");

            migrationBuilder.AlterColumn<int>(
                name: "UserDataId",
                table: "UserIdentity",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserIdentity_UserData_UserDataId",
                table: "UserIdentity");

            migrationBuilder.DropIndex(
                name: "IX_UserIdentity_UserDataId",
                table: "UserIdentity");

            migrationBuilder.AlterColumn<int>(
                name: "UserDataId",
                table: "UserIdentity",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserIdentity_UserDataId",
                table: "UserIdentity",
                column: "UserDataId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserIdentity_UserData_UserDataId",
                table: "UserIdentity",
                column: "UserDataId",
                principalTable: "UserData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
