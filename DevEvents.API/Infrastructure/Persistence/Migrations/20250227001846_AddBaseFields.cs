using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevEvents.API.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Speakers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Speakers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Registrations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Registrations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Conferences",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Conferences",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Attendees",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Attendees",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Speakers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Conferences");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Conferences");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Attendees");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Attendees");
        }
    }
}
