using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eryaz.Migrations
{
    public partial class UpdateDocumentEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AmbarId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MusteriId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_AmbarId",
                table: "Documents",
                column: "AmbarId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_MusteriId",
                table: "Documents",
                column: "MusteriId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Customers_MusteriId",
                table: "Documents",
                column: "MusteriId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Warehouses_AmbarId",
                table: "Documents",
                column: "AmbarId",
                principalTable: "Warehouses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Customers_MusteriId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Warehouses_AmbarId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_AmbarId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_MusteriId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "AmbarId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "MusteriId",
                table: "Documents");
        }
    }
}
