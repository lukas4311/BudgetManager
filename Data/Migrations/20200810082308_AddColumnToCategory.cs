using Microsoft.EntityFrameworkCore.Migrations;

namespace BudgetManager.Data.Migrations
{
    public partial class AddColumnToCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "PaymentCategory",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "PaymentCategory");
        }
    }
}
