using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ZeroORM.EFCore.Test.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttributedEntity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StringData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributedEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Conventioned",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AttributeMappedEntityId = table.Column<int>(nullable: false),
                    SomeString = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conventioned", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conventioned_AttributedEntity_AttributeMappedEntityId",
                        column: x => x.AttributeMappedEntityId,
                        principalTable: "AttributedEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SomeReallyUnrelatedName",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConventionMappedEntityId = table.Column<int>(nullable: false),
                    ReallyUnexpectedColumnName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SomeReallyUnrelatedName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SomeReallyUnrelatedName_Conventioned_ConventionMappedEntityId",
                        column: x => x.ConventionMappedEntityId,
                        principalTable: "Conventioned",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conventioned_AttributeMappedEntityId",
                table: "Conventioned",
                column: "AttributeMappedEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_SomeReallyUnrelatedName_ConventionMappedEntityId",
                table: "SomeReallyUnrelatedName",
                column: "ConventionMappedEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SomeReallyUnrelatedName");

            migrationBuilder.DropTable(
                name: "Conventioned");

            migrationBuilder.DropTable(
                name: "AttributedEntity");
        }
    }
}
