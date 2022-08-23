using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eryaz.Migrations
{
    public partial class UpdatedMovementEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Movements",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Movements");
        }
    }
}
