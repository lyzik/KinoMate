using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KinoMate.server.Migrations
{
    /// <inheritdoc />
    public partial class favoritesAndNotifiacions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    MoviesId = table.Column<string>(type: "TEXT", nullable: false),
                    SeriesId = table.Column<string>(type: "TEXT", nullable: false),
                    MoviesNotificationId = table.Column<string>(type: "TEXT", nullable: false),
                    SeriesNotificationId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favorites");
        }
    }
}
