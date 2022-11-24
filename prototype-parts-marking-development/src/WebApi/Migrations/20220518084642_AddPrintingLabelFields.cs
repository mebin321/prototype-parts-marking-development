using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    public partial class AddPrintingLabelFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GateLevel",
                table: "PrintingLabels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "PrintingLabels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaterialNumber",
                table: "PrintingLabels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Outlet",
                table: "PrintingLabels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectNumber",
                table: "PrintingLabels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisionCode",
                table: "PrintingLabels",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GateLevel",
                table: "PrintingLabels");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "PrintingLabels");

            migrationBuilder.DropColumn(
                name: "MaterialNumber",
                table: "PrintingLabels");

            migrationBuilder.DropColumn(
                name: "Outlet",
                table: "PrintingLabels");

            migrationBuilder.DropColumn(
                name: "ProjectNumber",
                table: "PrintingLabels");

            migrationBuilder.DropColumn(
                name: "RevisionCode",
                table: "PrintingLabels");
        }
    }
}
