using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevEvents.API.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToConference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Conferences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Conferences");
        }
    }
}
