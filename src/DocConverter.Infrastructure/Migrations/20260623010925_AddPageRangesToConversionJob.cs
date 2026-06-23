using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocConverter.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPageRangesToConversionJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EndPage",
                table: "ConversionJobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartPage",
                table: "ConversionJobs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndPage",
                table: "ConversionJobs");

            migrationBuilder.DropColumn(
                name: "StartPage",
                table: "ConversionJobs");
        }
    }
}
