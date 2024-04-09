using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    public partial class changedUserNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserNotifications_UserIdentity_UserId",
                table: "UserNotifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserNotifications",
                table: "UserNotifications");

            migrationBuilder.RenameTable(
                name: "UserNotifications",
                newName: "Notification");

            migrationBuilder.RenameColumn(
                name: "NotificationId",
                table: "Notification",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_UserNotifications_UserId",
                table: "Notification",
                newName: "IX_Notification_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_UserIdentity_UserId",
                table: "Notification",
                column: "UserId",
                principalTable: "UserIdentity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_UserIdentity_UserId",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "UserNotifications");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserNotifications",
                newName: "NotificationId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_UserId",
                table: "UserNotifications",
                newName: "IX_UserNotifications_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserNotifications",
                table: "UserNotifications",
                column: "NotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotifications_UserIdentity_UserId",
                table: "UserNotifications",
                column: "UserId",
                principalTable: "UserIdentity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
