using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eryaz.Migrations
{
    public partial class UpdateUnvanProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ünvan",
                table: "Customers",
                newName: "Unvan");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Unvan",
                table: "Customers",
                newName: "Ünvan");
        }
    }
}
