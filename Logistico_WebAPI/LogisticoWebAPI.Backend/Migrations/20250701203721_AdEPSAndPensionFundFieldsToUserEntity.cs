using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticoWebAPI.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AdEPSAndPensionFundFieldsToUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Eps",
                table: "AspNetUsers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PensionFund",
                table: "AspNetUsers",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Eps",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PensionFund",
                table: "AspNetUsers");
        }
    }
}
