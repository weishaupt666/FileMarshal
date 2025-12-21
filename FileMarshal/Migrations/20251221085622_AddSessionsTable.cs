using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileMarshal.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScanSessionId",
                table: "FileReports",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ScanSession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScanDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ScannedPath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScanSession", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileReports_ScanSessionId",
                table: "FileReports",
                column: "ScanSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileReports_ScanSession_ScanSessionId",
                table: "FileReports",
                column: "ScanSessionId",
                principalTable: "ScanSession",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileReports_ScanSession_ScanSessionId",
                table: "FileReports");

            migrationBuilder.DropTable(
                name: "ScanSession");

            migrationBuilder.DropIndex(
                name: "IX_FileReports_ScanSessionId",
                table: "FileReports");

            migrationBuilder.DropColumn(
                name: "ScanSessionId",
                table: "FileReports");
        }
    }
}
