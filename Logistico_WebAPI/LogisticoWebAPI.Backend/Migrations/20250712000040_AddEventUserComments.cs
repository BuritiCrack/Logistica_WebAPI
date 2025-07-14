using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticoWebAPI.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddEventUserComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminComments",
                table: "EventUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "EventUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserComments",
                table: "EventUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminComments",
                table: "EventUsers");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "EventUsers");

            migrationBuilder.DropColumn(
                name: "UserComments",
                table: "EventUsers");
        }
    }
}
