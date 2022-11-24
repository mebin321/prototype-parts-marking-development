﻿// <auto-generated />
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class GateLevelsMoniker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GateLevels_Code",
                table: "GateLevels");

            migrationBuilder.DropIndex(
                name: "IX_GateLevels_Title",
                table: "GateLevels");

            migrationBuilder.AddColumn<string>(
                name: "Moniker",
                table: "GateLevels",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GateLevels_Moniker",
                table: "GateLevels",
                column: "Moniker",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GateLevels_Moniker",
                table: "GateLevels");

            migrationBuilder.DropColumn(
                name: "Moniker",
                table: "GateLevels");

            migrationBuilder.CreateIndex(
                name: "IX_GateLevels_Code",
                table: "GateLevels",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GateLevels_Title",
                table: "GateLevels",
                column: "Title",
                unique: true);
        }
    }
}