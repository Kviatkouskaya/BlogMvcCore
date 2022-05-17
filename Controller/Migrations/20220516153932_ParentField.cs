using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogMvcCore.Migrations
{
    public partial class ParentField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Parent",
                table: "Comments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Parent",
                table: "Comments");
        }
    }
}
