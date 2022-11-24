﻿// <auto-generated />
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class CustomerProjectFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "PrototypeSets");

            migrationBuilder.AddColumn<string>(
                name: "Customer",
                table: "PrototypesPackages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Project",
                table: "PrototypesPackages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectNumber",
                table: "PrototypesPackages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Customer",
                table: "PrototypeSets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Project",
                table: "PrototypeSets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectNumber",
                table: "PrototypeSets",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Customer",
                table: "PrototypesPackages");

            migrationBuilder.DropColumn(
                name: "Project",
                table: "PrototypesPackages");

            migrationBuilder.DropColumn(
                name: "ProjectNumber",
                table: "PrototypesPackages");

            migrationBuilder.DropColumn(
                name: "Customer",
                table: "PrototypeSets");

            migrationBuilder.DropColumn(
                name: "Project",
                table: "PrototypeSets");

            migrationBuilder.DropColumn(
                name: "ProjectNumber",
                table: "PrototypeSets");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "PrototypeSets",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
