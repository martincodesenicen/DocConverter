using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocConverter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobSourceFilesRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "SourceFileId",
                table: "ConversionJobs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "ConversionJobSourceFiles",
                columns: table => new
                {
                    ConversionJobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoredFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SequenceOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversionJobSourceFiles", x => new { x.ConversionJobId, x.StoredFileId });
                    table.ForeignKey(
                        name: "FK_ConversionJobSourceFiles_ConversionJobs_ConversionJobId",
                        column: x => x.ConversionJobId,
                        principalTable: "ConversionJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConversionJobSourceFiles_StoredFiles_StoredFileId",
                        column: x => x.StoredFileId,
                        principalTable: "StoredFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConversionJobSourceFiles_StoredFileId",
                table: "ConversionJobSourceFiles",
                column: "StoredFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversionJobSourceFiles");

            migrationBuilder.AlterColumn<Guid>(
                name: "SourceFileId",
                table: "ConversionJobs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
