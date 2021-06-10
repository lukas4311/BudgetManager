using Microsoft.EntityFrameworkCore.Migrations;

namespace BudgetManager.Data.Migrations
{
    public partial class MoreConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TaxSetting_TaxType",
                table: "TaxSetting");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "UserData",
                fixedLength: true,
                maxLength: 9,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "UserData",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "UserData",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TaxType",
                table: "TaxSetting",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxSetting_TaxType",
                table: "TaxSetting",
                column: "TaxType",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TaxSetting_TaxType",
                table: "TaxSetting");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "UserData",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldFixedLength: true,
                oldMaxLength: 9);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "UserData",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "UserData",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "TaxType",
                table: "TaxSetting",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_TaxSetting_TaxType",
                table: "TaxSetting",
                column: "TaxType",
                unique: true,
                filter: "[TaxType] IS NOT NULL");
        }
    }
}
