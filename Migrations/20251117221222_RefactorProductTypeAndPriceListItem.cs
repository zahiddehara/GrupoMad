using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrupoMad.Migrations
{
    /// <inheritdoc />
    public partial class RefactorProductTypeAndPriceListItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Eliminar columna PricingType de Products
            migrationBuilder.DropColumn(
                name: "PricingType",
                table: "Products");

            // 2. Agregar columna Price a PriceListItems ANTES de eliminar las antiguas
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "PriceListItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            // 3. Migrar datos de precios: copiar el primer precio no-nulo encontrado
            migrationBuilder.Sql(@"
                UPDATE PriceListItems
                SET Price = COALESCE(PricePerSquareMeter, PricePerUnit, PricePerLinearMeter, 0)
                WHERE Price = 0;
            ");

            // 4. Agregar columna Variant
            migrationBuilder.AddColumn<string>(
                name: "Variant",
                table: "PriceListItems",
                type: "TEXT",
                nullable: true);

            // 5. Ahora eliminar las columnas antiguas de precios
            migrationBuilder.DropColumn(
                name: "PricePerLinearMeter",
                table: "PriceListItems");

            migrationBuilder.DropColumn(
                name: "PricePerSquareMeter",
                table: "PriceListItems");

            migrationBuilder.DropColumn(
                name: "PricePerUnit",
                table: "PriceListItems");

            // 6. Crear tabla ProductTypes
            migrationBuilder.CreateTable(
                name: "ProductTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    PricingType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypes", x => x.Id);
                });

            // 7. Insertar ProductType "Persiana" con Id=1 para productos existentes
            migrationBuilder.Sql(@"
                INSERT INTO ProductTypes (Id, Name, Description, PricingType, IsActive, CreatedAt)
                VALUES (1, 'Persiana', 'Persianas', 0, 1, datetime('now'));
            ");

            // 8. Renombrar ProductType a ProductTypeId en Products
            migrationBuilder.RenameColumn(
                name: "ProductType",
                table: "Products",
                newName: "ProductTypeId");

            // 9. Crear índice
            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductTypeId",
                table: "Products",
                column: "ProductTypeId");

            // 10. Agregar Foreign Key
            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductTypes_ProductTypeId",
                table: "Products",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductTypes_ProductTypeId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductTypes");

            migrationBuilder.DropIndex(
                name: "IX_Products_ProductTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "PriceListItems");

            migrationBuilder.DropColumn(
                name: "Variant",
                table: "PriceListItems");

            migrationBuilder.RenameColumn(
                name: "ProductTypeId",
                table: "Products",
                newName: "ProductType");

            migrationBuilder.AddColumn<int>(
                name: "PricingType",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerLinearMeter",
                table: "PriceListItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerSquareMeter",
                table: "PriceListItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerUnit",
                table: "PriceListItems",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
