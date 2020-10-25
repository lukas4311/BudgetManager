using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class AddNameColumnToBudget : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Budget",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Budget_Name",
                table: "Budget",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Budget_Name",
                table: "Budget");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Budget");
        }
    }
}
