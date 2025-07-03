using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticoWebAPI.Backend.Migrations
{
    /// <inheritdoc />
    public partial class FixConflictWithUserIdPropertyInEventUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_AspNetUsers_UserId1",
                table: "EventUser");

            migrationBuilder.DropIndex(
                name: "IX_EventUser_UserId1",
                table: "EventUser");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "EventUser");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "EventUser",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_EventUser_UserId",
                table: "EventUser",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_AspNetUsers_UserId",
                table: "EventUser",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_AspNetUsers_UserId",
                table: "EventUser");

            migrationBuilder.DropIndex(
                name: "IX_EventUser_UserId",
                table: "EventUser");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "EventUser",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "EventUser",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventUser_UserId1",
                table: "EventUser",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_AspNetUsers_UserId1",
                table: "EventUser",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
