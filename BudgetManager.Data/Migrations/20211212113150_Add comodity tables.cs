using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BudgetManager.Data.Migrations
{
    public partial class Addcomoditytables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComodityUnit",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 20, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComodityUnit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComodityType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(maxLength: 20, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    ComodityUnitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComodityType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComodityType_ComodityUnit_ComodityUnitId",
                        column: x => x.ComodityUnitId,
                        principalTable: "ComodityUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComodityTradeHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradeTimeStamp = table.Column<DateTime>(nullable: false),
                    ComodityTypeId = table.Column<int>(nullable: false),
                    TradeSize = table.Column<double>(nullable: false),
                    TradeValue = table.Column<double>(nullable: false),
                    CurrencySymbolId = table.Column<int>(nullable: false),
                    UserIdentityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComodityTradeHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComodityTradeHistory_ComodityType_ComodityTypeId",
                        column: x => x.ComodityTypeId,
                        principalTable: "ComodityType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComodityTradeHistory_CurrencySymbol_CurrencySymbolId",
                        column: x => x.CurrencySymbolId,
                        principalTable: "CurrencySymbol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComodityTradeHistory_UserIdentity_UserIdentityId",
                        column: x => x.UserIdentityId,
                        principalTable: "UserIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComodityTradeHistory_ComodityTypeId",
                table: "ComodityTradeHistory",
                column: "ComodityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ComodityTradeHistory_CurrencySymbolId",
                table: "ComodityTradeHistory",
                column: "CurrencySymbolId");

            migrationBuilder.CreateIndex(
                name: "IX_ComodityTradeHistory_UserIdentityId",
                table: "ComodityTradeHistory",
                column: "UserIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_ComodityType_Code",
                table: "ComodityType",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ComodityType_ComodityUnitId",
                table: "ComodityType",
                column: "ComodityUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ComodityUnit_Code",
                table: "ComodityUnit",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComodityTradeHistory");

            migrationBuilder.DropTable(
                name: "ComodityType");

            migrationBuilder.DropTable(
                name: "ComodityUnit");
        }
    }
}
