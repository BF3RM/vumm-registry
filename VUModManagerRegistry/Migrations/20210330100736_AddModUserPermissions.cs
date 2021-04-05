using Microsoft.EntityFrameworkCore.Migrations;

namespace VUModManagerRegistry.Migrations
{
    public partial class AddModUserPermissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModUserPermissions",
                columns: table => new
                {
                    ModId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Permission = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModUserPermissions", x => new { x.ModId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ModUserPermissions_Mods_ModId",
                        column: x => x.ModId,
                        principalTable: "Mods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModUserPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModUserPermissions_UserId",
                table: "ModUserPermissions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModUserPermissions");
        }
    }
}
