using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    public partial class AddStockSplit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockSplit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockTickerId = table.Column<int>(type: "int", nullable: false),
                    SplitTextInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SplitCoefficient = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockSplit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockSplit_StockTicker_StockTickerId",
                        column: x => x.StockTickerId,
                        principalTable: "StockTicker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockSplit_StockTickerId",
                table: "StockSplit",
                column: "StockTickerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockSplit");
        }
    }
}
