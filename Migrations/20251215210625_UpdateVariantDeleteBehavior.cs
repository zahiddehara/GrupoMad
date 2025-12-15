using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrupoMad.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVariantDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceListItems_ProductTypeVariants_ProductTypeVariantId",
                table: "PriceListItems");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceListItems_ProductTypeVariants_ProductTypeVariantId",
                table: "PriceListItems",
                column: "ProductTypeVariantId",
                principalTable: "ProductTypeVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceListItems_ProductTypeVariants_ProductTypeVariantId",
                table: "PriceListItems");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceListItems_ProductTypeVariants_ProductTypeVariantId",
                table: "PriceListItems",
                column: "ProductTypeVariantId",
                principalTable: "ProductTypeVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
