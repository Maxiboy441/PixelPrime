using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class optmise_api_requests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "movie_poster",
                table: "Watchlists",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "movie_title",
                table: "Watchlists",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "movie_poster",
                table: "Reviews",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "movie_title",
                table: "Reviews",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "movie_poster",
                table: "Recommendations",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "movie_title",
                table: "Recommendations",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "movie_poster",
                table: "Ratings",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "movie_title",
                table: "Ratings",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "movie_poster",
                table: "Favorites",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "movie_title",
                table: "Favorites",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "movie_poster",
                table: "Watchlists");

            migrationBuilder.DropColumn(
                name: "movie_title",
                table: "Watchlists");

            migrationBuilder.DropColumn(
                name: "movie_poster",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "movie_title",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "movie_poster",
                table: "Recommendations");

            migrationBuilder.DropColumn(
                name: "movie_title",
                table: "Recommendations");

            migrationBuilder.DropColumn(
                name: "movie_poster",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "movie_title",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "movie_poster",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "movie_title",
                table: "Favorites");
        }
    }
}
