using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    public partial class RenamedUserIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_UserIdentity_UserId",
                table: "Notification");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Notification",
                newName: "UserIdentityId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_UserId",
                table: "Notification",
                newName: "IX_Notification_UserIdentityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_UserIdentity_UserIdentityId",
                table: "Notification",
                column: "UserIdentityId",
                principalTable: "UserIdentity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_UserIdentity_UserIdentityId",
                table: "Notification");

            migrationBuilder.RenameColumn(
                name: "UserIdentityId",
                table: "Notification",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_UserIdentityId",
                table: "Notification",
                newName: "IX_Notification_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_UserIdentity_UserId",
                table: "Notification",
                column: "UserId",
                principalTable: "UserIdentity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
