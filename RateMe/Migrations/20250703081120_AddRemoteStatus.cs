using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RateMe.Migrations
{
    /// <inheritdoc />
    public partial class AddRemoteStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemoteStatus",
                table: "Subjects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RemoteStatus",
                table: "Elements",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemoteStatus",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "RemoteStatus",
                table: "Elements");
        }
    }
}
