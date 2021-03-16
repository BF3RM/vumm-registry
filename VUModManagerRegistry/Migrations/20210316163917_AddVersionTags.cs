using Microsoft.EntityFrameworkCore.Migrations;

namespace VUModManagerRegistry.Migrations
{
    public partial class AddVersionTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModVersions_Mods_ModId",
                table: "ModVersions");

            migrationBuilder.AlterColumn<long>(
                name: "ModId",
                table: "ModVersions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "ModVersions",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ModVersions_Mods_ModId",
                table: "ModVersions",
                column: "ModId",
                principalTable: "Mods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModVersions_Mods_ModId",
                table: "ModVersions");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "ModVersions");

            migrationBuilder.AlterColumn<long>(
                name: "ModId",
                table: "ModVersions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_ModVersions_Mods_ModId",
                table: "ModVersions",
                column: "ModId",
                principalTable: "Mods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
