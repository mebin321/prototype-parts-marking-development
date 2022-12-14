// <auto-generated />
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WebApi.Migrations
{
    public partial class AddPrototypeCounters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrototypeCounters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LocationId = table.Column<int>(nullable: false),
                    EvidenceYearId = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrototypeCounters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrototypeCounters_EvidenceYears_EvidenceYearId",
                        column: x => x.EvidenceYearId,
                        principalTable: "EvidenceYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrototypeCounters_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrototypeCounters_EvidenceYearId",
                table: "PrototypeCounters",
                column: "EvidenceYearId");

            migrationBuilder.CreateIndex(
                name: "IX_PrototypeCounters_LocationId_EvidenceYearId",
                table: "PrototypeCounters",
                columns: new[] { "LocationId", "EvidenceYearId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrototypeCounters");
        }
    }
}
