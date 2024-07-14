using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBrokerOfReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrokerId",
                table: "BrokerReportToProcess",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BrokerReportToProcess_BrokerId",
                table: "BrokerReportToProcess",
                column: "BrokerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BrokerReportToProcess_EnumItem_BrokerId",
                table: "BrokerReportToProcess",
                column: "BrokerId",
                principalTable: "EnumItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrokerReportToProcess_EnumItem_BrokerId",
                table: "BrokerReportToProcess");

            migrationBuilder.DropIndex(
                name: "IX_BrokerReportToProcess_BrokerId",
                table: "BrokerReportToProcess");

            migrationBuilder.DropColumn(
                name: "BrokerId",
                table: "BrokerReportToProcess");
        }
    }
}
