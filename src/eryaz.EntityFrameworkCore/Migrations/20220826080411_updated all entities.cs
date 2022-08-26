using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eryaz.Migrations
{
    public partial class updatedallentities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Documents_DocumentId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Products_ProductId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Warehouses_WarehouseId",
                table: "Movements");

            migrationBuilder.DropTable(
                name: "DocumentMovementStatuses");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Movements_DocumentId",
                table: "Movements");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "Movements");

            migrationBuilder.AlterColumn<int>(
                name: "WarehouseId",
                table: "Movements",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Movements",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentHeaders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DetailDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DocumentHeaderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentDetails_DocumentHeaders_DocumentHeaderId",
                        column: x => x.DocumentHeaderId,
                        principalTable: "DocumentHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDetails_DocumentHeaderId",
                table: "DocumentDetails",
                column: "DocumentHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDetails_ProductId",
                table: "DocumentDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentHeaders_CustomerId",
                table: "DocumentHeaders",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Products_ProductId",
                table: "Movements",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Warehouses_WarehouseId",
                table: "Movements",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Products_ProductId",
                table: "Movements");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Warehouses_WarehouseId",
                table: "Movements");

            migrationBuilder.DropTable(
                name: "DocumentDetails");

            migrationBuilder.DropTable(
                name: "DocumentHeaders");

            migrationBuilder.AlterColumn<int>(
                name: "WarehouseId",
                table: "Movements",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Movements",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "Movements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentMovementStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovementId = table.Column<int>(type: "int", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentMovementStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentMovementStatuses_Movements_MovementId",
                        column: x => x.MovementId,
                        principalTable: "Movements",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movements_DocumentId",
                table: "Movements",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentMovementStatuses_MovementId",
                table: "DocumentMovementStatuses",
                column: "MovementId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CustomerId",
                table: "Documents",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Documents_DocumentId",
                table: "Movements",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Products_ProductId",
                table: "Movements",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Warehouses_WarehouseId",
                table: "Movements",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id");
        }
    }
}
