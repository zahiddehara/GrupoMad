using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrupoMad.Migrations
{
    /// <inheritdoc />
    public partial class RefactorVariantToUseId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Paso 1: Agregar la nueva columna ProductTypeVariantId (nullable)
            migrationBuilder.AddColumn<int>(
                name: "ProductTypeVariantId",
                table: "PriceListItems",
                type: "INTEGER",
                nullable: true);

            // Paso 2: Migrar datos existentes - convertir nombres de variantes a IDs
            migrationBuilder.Sql(@"
                UPDATE PriceListItems
                SET ProductTypeVariantId = (
                    SELECT Id
                    FROM ProductTypeVariants
                    WHERE ProductTypeVariants.Name = PriceListItems.Variant
                    LIMIT 1
                )
                WHERE PriceListItems.Variant IS NOT NULL
                  AND PriceListItems.Variant != ''
            ");

            // Paso 3: Eliminar la columna antigua Variant
            migrationBuilder.DropColumn(
                name: "Variant",
                table: "PriceListItems");

            // Paso 4: Crear índice y Foreign Key
            migrationBuilder.CreateIndex(
                name: "IX_PriceListItems_ProductTypeVariantId",
                table: "PriceListItems",
                column: "ProductTypeVariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceListItems_ProductTypeVariants_ProductTypeVariantId",
                table: "PriceListItems",
                column: "ProductTypeVariantId",
                principalTable: "ProductTypeVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Paso 1: Eliminar Foreign Key e índice
            migrationBuilder.DropForeignKey(
                name: "FK_PriceListItems_ProductTypeVariants_ProductTypeVariantId",
                table: "PriceListItems");

            migrationBuilder.DropIndex(
                name: "IX_PriceListItems_ProductTypeVariantId",
                table: "PriceListItems");

            // Paso 2: Agregar la columna Variant antigua
            migrationBuilder.AddColumn<string>(
                name: "Variant",
                table: "PriceListItems",
                type: "TEXT",
                nullable: true);

            // Paso 3: Migrar datos de vuelta - convertir IDs a nombres de variantes
            migrationBuilder.Sql(@"
                UPDATE PriceListItems
                SET Variant = (
                    SELECT Name
                    FROM ProductTypeVariants
                    WHERE ProductTypeVariants.Id = PriceListItems.ProductTypeVariantId
                )
                WHERE PriceListItems.ProductTypeVariantId IS NOT NULL
            ");

            // Paso 4: Eliminar la columna ProductTypeVariantId
            migrationBuilder.DropColumn(
                name: "ProductTypeVariantId",
                table: "PriceListItems");
        }
    }
}
