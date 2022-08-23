using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eryaz.Migrations
{
    public partial class UpdateEntityProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "AmbarId",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "AmbarTipi",
                table: "Warehouses",
                newName: "WarehouseType");

            migrationBuilder.RenameColumn(
                name: "AmbarKodu",
                table: "Warehouses",
                newName: "WarehouseName");

            migrationBuilder.RenameColumn(
                name: "AmbarAdi",
                table: "Warehouses",
                newName: "WarehouseCode");

            migrationBuilder.RenameColumn(
                name: "StokKodu",
                table: "Products",
                newName: "ProductName");

            migrationBuilder.RenameColumn(
                name: "StokAdi",
                table: "Products",
                newName: "ProductCode");

            migrationBuilder.RenameColumn(
                name: "Marka",
                table: "Products",
                newName: "Brand");

            migrationBuilder.RenameColumn(
                name: "MusteriId",
                table: "Documents",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "KayitTarihi",
                table: "Documents",
                newName: "RegistrationDate");

            migrationBuilder.RenameColumn(
                name: "EvrakTarihi",
                table: "Documents",
                newName: "DocumentDate");

            migrationBuilder.RenameColumn(
                name: "EvrakNumarasi",
                table: "Documents",
                newName: "DocumentNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_MusteriId",
                table: "Documents",
                newName: "IX_Documents_CustomerId");

            migrationBuilder.RenameColumn(
                name: "VergiNo",
                table: "Customers",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "VergiDaire",
                table: "Customers",
                newName: "Telephone");

            migrationBuilder.RenameColumn(
                name: "Unvan",
                table: "Customers",
                newName: "TaxOffice");

            migrationBuilder.RenameColumn(
                name: "Telefon",
                table: "Customers",
                newName: "TaxNo");

            migrationBuilder.RenameColumn(
                name: "Eposta",
                table: "Customers",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "CariKod",
                table: "Customers",
                newName: "CustomerCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Customers_CustomerId",
                table: "Documents",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Customers_CustomerId",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "WarehouseType",
                table: "Warehouses",
                newName: "AmbarTipi");

            migrationBuilder.RenameColumn(
                name: "WarehouseName",
                table: "Warehouses",
                newName: "AmbarKodu");

            migrationBuilder.RenameColumn(
                name: "WarehouseCode",
                table: "Warehouses",
                newName: "AmbarAdi");

            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "Products",
                newName: "StokKodu");

            migrationBuilder.RenameColumn(
                name: "ProductCode",
                table: "Products",
                newName: "StokAdi");

            migrationBuilder.RenameColumn(
                name: "Brand",
                table: "Products",
                newName: "Marka");

            migrationBuilder.RenameColumn(
                name: "RegistrationDate",
                table: "Documents",
                newName: "KayitTarihi");

            migrationBuilder.RenameColumn(
                name: "DocumentNumber",
                table: "Documents",
                newName: "EvrakNumarasi");

            migrationBuilder.RenameColumn(
                name: "DocumentDate",
                table: "Documents",
                newName: "EvrakTarihi");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Documents",
                newName: "MusteriId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_CustomerId",
                table: "Documents",
                newName: "IX_Documents_MusteriId");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Customers",
                newName: "VergiNo");

            migrationBuilder.RenameColumn(
                name: "Telephone",
                table: "Customers",
                newName: "VergiDaire");

            migrationBuilder.RenameColumn(
                name: "TaxOffice",
                table: "Customers",
                newName: "Unvan");

            migrationBuilder.RenameColumn(
                name: "TaxNo",
                table: "Customers",
                newName: "Telefon");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Customers",
                newName: "Eposta");

            migrationBuilder.RenameColumn(
                name: "CustomerCode",
                table: "Customers",
                newName: "CariKod");

            migrationBuilder.AddColumn<int>(
                name: "AmbarId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_AmbarId",
                table: "Documents",
                column: "AmbarId");

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
    }
}
