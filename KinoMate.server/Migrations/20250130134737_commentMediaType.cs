using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KinoMate.server.Migrations
{
    /// <inheritdoc />
    public partial class commentMediaType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MediaType",
                table: "Comments",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "Comments");
        }
    }
}
