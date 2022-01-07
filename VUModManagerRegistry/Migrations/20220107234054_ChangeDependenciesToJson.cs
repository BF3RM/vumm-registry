using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VUModManagerRegistry.Migrations
{
    public partial class ChangeDependenciesToJson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            ALTER TABLE ""ModVersions"" ALTER COLUMN ""Dependencies"" TYPE jsonb USING ""Dependencies""::jsonb;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Dependencies",
                table: "ModVersions",
                type: "text",
                nullable: true,
                oldClrType: typeof(Dictionary<string, string>),
                oldType: "jsonb",
                oldNullable: true);
        }
    }
}
