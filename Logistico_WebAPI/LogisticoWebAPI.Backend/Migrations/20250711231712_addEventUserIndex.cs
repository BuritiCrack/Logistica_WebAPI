using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticoWebAPI.Backend.Migrations
{
    /// <inheritdoc />
    public partial class addEventUserIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventUsers_EventId",
                table: "EventUsers");

            migrationBuilder.DropColumn(
                name: "IsUserConfirmed",
                table: "EventUsers");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EventUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EventUsers_EventId_UserId",
                table: "EventUsers",
                columns: new[] { "EventId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventUsers_EventId_UserId",
                table: "EventUsers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EventUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsUserConfirmed",
                table: "EventUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_EventUsers_EventId",
                table: "EventUsers",
                column: "EventId");
        }
    }
}
