using Microsoft.EntityFrameworkCore.Migrations;

namespace AndreyevInterview.Migrations
{
    public partial class add_isBillable_Col_LineItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isBillable",
                table: "LineItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isBillable",
                table: "LineItems");
        }
    }
}
