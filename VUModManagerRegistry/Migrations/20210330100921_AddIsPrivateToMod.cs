using Microsoft.EntityFrameworkCore.Migrations;

namespace VUModManagerRegistry.Migrations
{
    public partial class AddIsPrivateToMod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Mods",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Mods");
        }
    }
}
