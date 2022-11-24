﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

namespace WebApi.Migrations
{
    public partial class GlobalProjectIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "SearchVector",
                table: "GlobalProjects",
                type: "tsvector",
                nullable: true)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "ProjectNumber", "Description", "Customer" });

            migrationBuilder.CreateIndex(
                name: "IX_GlobalProjects_Customer",
                table: "GlobalProjects",
                column: "Customer");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalProjects_ProjectNumber",
                table: "GlobalProjects",
                column: "ProjectNumber");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalProjects_SearchVector",
                table: "GlobalProjects",
                column: "SearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GlobalProjects_Customer",
                table: "GlobalProjects");

            migrationBuilder.DropIndex(
                name: "IX_GlobalProjects_ProjectNumber",
                table: "GlobalProjects");

            migrationBuilder.DropIndex(
                name: "IX_GlobalProjects_SearchVector",
                table: "GlobalProjects");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "GlobalProjects");
        }
    }
}
