using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalDigitalVault.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentSha256Hash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sha256Hash",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sha256Hash",
                table: "Documents");
        }
    }
}
