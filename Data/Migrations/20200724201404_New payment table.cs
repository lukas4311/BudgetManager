using Microsoft.EntityFrameworkCore.Migrations;

namespace BudgetManager.Data.Migrations
{
    public partial class Newpaymenttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MyProperty",
                table: "Payment");

            migrationBuilder.AddColumn<int>(
                name: "PaymentCategoryId",
                table: "Payment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentTypeId",
                table: "Payment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpeningBalance",
                table: "BankAccount",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PaymentCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PaymentCategoryId",
                table: "Payment",
                column: "PaymentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PaymentTypeId",
                table: "Payment",
                column: "PaymentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_PaymentCategory_PaymentCategoryId",
                table: "Payment",
                column: "PaymentCategoryId",
                principalTable: "PaymentCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_PaymentType_PaymentTypeId",
                table: "Payment",
                column: "PaymentTypeId",
                principalTable: "PaymentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_PaymentCategory_PaymentCategoryId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_PaymentType_PaymentTypeId",
                table: "Payment");

            migrationBuilder.DropTable(
                name: "PaymentCategory");

            migrationBuilder.DropTable(
                name: "PaymentType");

            migrationBuilder.DropIndex(
                name: "IX_Payment_PaymentCategoryId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_PaymentTypeId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PaymentCategoryId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PaymentTypeId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "OpeningBalance",
                table: "BankAccount");

            migrationBuilder.AddColumn<int>(
                name: "MyProperty",
                table: "Payment",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
