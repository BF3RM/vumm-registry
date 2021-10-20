using Microsoft.EntityFrameworkCore.Migrations;

namespace VUModManagerRegistry.Migrations
{
    public partial class AddTagToModUserPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ModUserPermissions",
                table: "ModUserPermissions");

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "ModUserPermissions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModUserPermissions",
                table: "ModUserPermissions",
                columns: new[] { "ModId", "UserId", "Tag" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ModUserPermissions",
                table: "ModUserPermissions");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "ModUserPermissions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModUserPermissions",
                table: "ModUserPermissions",
                columns: new[] { "ModId", "UserId" });
        }
    }
}
