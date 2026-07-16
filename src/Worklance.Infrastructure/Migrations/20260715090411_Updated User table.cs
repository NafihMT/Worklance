using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Worklance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUsertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AadhaarProofPath",
                table: "Users");

            migrationBuilder.AddColumn<byte[]>(
                name: "AadhaarImageBytes",
                table: "Users",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsOcrMatched",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AadhaarImageBytes",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsOcrMatched",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "AadhaarProofPath",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
