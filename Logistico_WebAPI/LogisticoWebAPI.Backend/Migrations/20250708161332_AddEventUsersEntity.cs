using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticoWebAPI.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddEventUsersEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_AspNetUsers_UserId",
                table: "EventUser");

            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_Events_EventId",
                table: "EventUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventUser",
                table: "EventUser");

            migrationBuilder.RenameTable(
                name: "EventUser",
                newName: "EventUsers");

            migrationBuilder.RenameColumn(
                name: "FechaHoraInicio",
                table: "Events",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "FechaHoraFin",
                table: "Events",
                newName: "EndDate");

            migrationBuilder.RenameIndex(
                name: "IX_Events_FechaHoraInicio_Name",
                table: "Events",
                newName: "IX_Events_StartDate_Name");

            migrationBuilder.RenameIndex(
                name: "IX_EventUser_UserId",
                table: "EventUsers",
                newName: "IX_EventUsers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventUser_EventId",
                table: "EventUsers",
                newName: "IX_EventUsers_EventId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Events",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventUsers",
                table: "EventUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventUsers_AspNetUsers_UserId",
                table: "EventUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventUsers_Events_EventId",
                table: "EventUsers",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventUsers_AspNetUsers_UserId",
                table: "EventUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_EventUsers_Events_EventId",
                table: "EventUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventUsers",
                table: "EventUsers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Events");

            migrationBuilder.RenameTable(
                name: "EventUsers",
                newName: "EventUser");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Events",
                newName: "FechaHoraInicio");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Events",
                newName: "FechaHoraFin");

            migrationBuilder.RenameIndex(
                name: "IX_Events_StartDate_Name",
                table: "Events",
                newName: "IX_Events_FechaHoraInicio_Name");

            migrationBuilder.RenameIndex(
                name: "IX_EventUsers_UserId",
                table: "EventUser",
                newName: "IX_EventUser_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventUsers_EventId",
                table: "EventUser",
                newName: "IX_EventUser_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventUser",
                table: "EventUser",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_AspNetUsers_UserId",
                table: "EventUser",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_Events_EventId",
                table: "EventUser",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
