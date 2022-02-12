using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BudgetManager.Data.Migrations
{
    public partial class OtherInvestment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OtherInvestment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OpeningBalance = table.Column<int>(nullable: false),
                    UserIdentityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherInvestment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtherInvestment_UserIdentity_UserIdentityId",
                        column: x => x.UserIdentityId,
                        principalTable: "UserIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OtherInvestmentBalaceHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    Balance = table.Column<int>(nullable: false),
                    OtherInvestmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherInvestmentBalaceHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtherInvestmentBalaceHistory_OtherInvestment_OtherInvestmentId",
                        column: x => x.OtherInvestmentId,
                        principalTable: "OtherInvestment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OtherInvestmentTag",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OtherInvestmentId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherInvestmentTag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtherInvestmentTag_OtherInvestment_OtherInvestmentId",
                        column: x => x.OtherInvestmentId,
                        principalTable: "OtherInvestment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OtherInvestmentTag_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OtherInvestment_Code",
                table: "OtherInvestment",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OtherInvestment_UserIdentityId",
                table: "OtherInvestment",
                column: "UserIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherInvestmentBalaceHistory_OtherInvestmentId",
                table: "OtherInvestmentBalaceHistory",
                column: "OtherInvestmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherInvestmentTag_OtherInvestmentId",
                table: "OtherInvestmentTag",
                column: "OtherInvestmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherInvestmentTag_TagId",
                table: "OtherInvestmentTag",
                column: "TagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OtherInvestmentBalaceHistory");

            migrationBuilder.DropTable(
                name: "OtherInvestmentTag");

            migrationBuilder.DropTable(
                name: "OtherInvestment");
        }
    }
}
