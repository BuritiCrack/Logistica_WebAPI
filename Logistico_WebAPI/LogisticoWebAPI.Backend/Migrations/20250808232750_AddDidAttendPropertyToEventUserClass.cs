using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticoWebAPI.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddDidAttendPropertyToEventUserClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsEventCancelled",
                table: "EventUsers",
                newName: "DidAttend");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DidAttend",
                table: "EventUsers",
                newName: "IsEventCancelled");
        }
    }
}
