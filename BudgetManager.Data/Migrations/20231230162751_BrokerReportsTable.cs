using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetManager.Data.Migrations
{
    public partial class BrokerReportsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrokerReportToProcessState",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokerReportToProcessState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BrokerReportType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokerReportType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BrokerReportToProcess",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileContentBase64 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrokerReportToProcessStateId = table.Column<int>(type: "int", nullable: false),
                    UserIdentityId = table.Column<int>(type: "int", nullable: false),
                    BrokerReportTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrokerReportToProcess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrokerReportToProcess_BrokerReportToProcessState_BrokerReportToProcessStateId",
                        column: x => x.BrokerReportToProcessStateId,
                        principalTable: "BrokerReportToProcessState",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrokerReportToProcess_BrokerReportType_BrokerReportTypeId",
                        column: x => x.BrokerReportTypeId,
                        principalTable: "BrokerReportType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrokerReportToProcess_UserIdentity_UserIdentityId",
                        column: x => x.UserIdentityId,
                        principalTable: "UserIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrokerReportToProcess_BrokerReportToProcessStateId",
                table: "BrokerReportToProcess",
                column: "BrokerReportToProcessStateId");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerReportToProcess_BrokerReportTypeId",
                table: "BrokerReportToProcess",
                column: "BrokerReportTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerReportToProcess_UserIdentityId",
                table: "BrokerReportToProcess",
                column: "UserIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerReportToProcessState_Code",
                table: "BrokerReportToProcessState",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BrokerReportType_Code",
                table: "BrokerReportType",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrokerReportToProcess");

            migrationBuilder.DropTable(
                name: "BrokerReportToProcessState");

            migrationBuilder.DropTable(
                name: "BrokerReportType");
        }
    }
}
