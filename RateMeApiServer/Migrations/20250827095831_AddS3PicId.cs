using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RateMeApiServer.Migrations
{
    /// <inheritdoc />
    public partial class AddS3PicId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "S3PicId",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "S3PicId",
                table: "Users");
        }
    }
}
