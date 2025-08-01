using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticoWebAPI.Backend.Migrations
{
    /// <inheritdoc />
    public partial class FixDateFormatgEventEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserComments",
                table: "EventUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserComments",
                table: "EventUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
