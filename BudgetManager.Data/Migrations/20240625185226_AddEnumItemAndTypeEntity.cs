using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    public partial class AddEnumItemAndTypeEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EnumItemType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnumItemType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnumItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EnumItemTypeId = table.Column<int>(type: "int", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnumItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnumItem_EnumItemType_EnumItemTypeId",
                        column: x => x.EnumItemTypeId,
                        principalTable: "EnumItemType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnumItem_Code",
                table: "EnumItem",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnumItem_EnumItemTypeId",
                table: "EnumItem",
                column: "EnumItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EnumItemType_Code",
                table: "EnumItemType",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnumItem");

            migrationBuilder.DropTable(
                name: "EnumItemType");
        }
    }
}
