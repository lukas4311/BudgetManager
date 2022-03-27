using Microsoft.EntityFrameworkCore.Migrations;

namespace BudgetManager.Data.Migrations
{
    public partial class addressAndCompanyProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Country = table.Column<string>(maxLength: 50, nullable: false),
                    City = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Zip = table.Column<string>(nullable: true),
                    Lat = table.Column<decimal>(nullable: true),
                    Lng = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyProfile",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(maxLength: 10, nullable: false),
                    CompanyName = table.Column<string>(maxLength: 50, nullable: false),
                    Currency = table.Column<string>(maxLength: 10, nullable: false),
                    Isin = table.Column<string>(nullable: true),
                    ExchangeShortName = table.Column<string>(nullable: true),
                    Industry = table.Column<string>(nullable: true),
                    Website = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Sector = table.Column<string>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    AddressId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyProfile_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfile_AddressId",
                table: "CompanyProfile",
                column: "AddressId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyProfile");

            migrationBuilder.DropTable(
                name: "Address");
        }
    }
}
