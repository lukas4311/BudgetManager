using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class changeEnumItemIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EnumItem_Code",
                table: "EnumItem");

            migrationBuilder.CreateIndex(
                name: "IX_EnumItem_Code_EnumItemTypeId",
                table: "EnumItem",
                columns: new[] { "Code", "EnumItemTypeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EnumItem_Code_EnumItemTypeId",
                table: "EnumItem");

            migrationBuilder.CreateIndex(
                name: "IX_EnumItem_Code",
                table: "EnumItem",
                column: "Code",
                unique: true);
        }
    }
}
