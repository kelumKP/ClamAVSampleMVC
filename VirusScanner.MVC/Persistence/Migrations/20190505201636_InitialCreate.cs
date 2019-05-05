using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VirusScanner.MVC.Persistence.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    Alias = table.Column<string>(maxLength: 256, nullable: false),
                    Region = table.Column<string>(maxLength: 24, nullable: false),
                    Bucket = table.Column<string>(maxLength: 64, nullable: false),
                    ContentType = table.Column<string>(maxLength: 100, nullable: true),
                    Size = table.Column<long>(nullable: false),
                    Uploaded = table.Column<DateTime>(type: "datetime", nullable: false),
                    ScanResult = table.Column<string>(maxLength: 16, nullable: false),
                    Infected = table.Column<bool>(nullable: false),
                    Scanned = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Viruses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FileId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Viruses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Viruses_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Viruses_FileId",
                table: "Viruses",
                column: "FileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Viruses");

            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}
