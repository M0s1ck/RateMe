using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RateMeApiServer.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCurriculumYearQuote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Curriculum",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Quote",
                table: "Users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Curriculum",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Quote",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Users");
        }
    }
}
