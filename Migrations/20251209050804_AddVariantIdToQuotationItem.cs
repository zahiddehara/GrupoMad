using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrupoMad.Migrations
{
    /// <inheritdoc />
    public partial class AddVariantIdToQuotationItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductTypeVariantId",
                table: "QuotationItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE QuotationItems
                SET ProductTypeVariantId = (
                    SELECT Id
                    FROM ProductTypeVariants
                    WHERE ProductTypeVariants.Name = QuotationItems.Variant
                    LIMIT 1
                )
                WHERE QuotationItems.Variant IS NOT NULL
                  AND QuotationItems.Variant != ''
                  AND QuotationItems.QuotationId IN (
                      SELECT Id FROM Quotations WHERE Status = 0
                  )
            ");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItems_ProductTypeVariantId",
                table: "QuotationItems",
                column: "ProductTypeVariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationItems_ProductTypeVariants_ProductTypeVariantId",
                table: "QuotationItems",
                column: "ProductTypeVariantId",
                principalTable: "ProductTypeVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationItems_ProductTypeVariants_ProductTypeVariantId",
                table: "QuotationItems");

            migrationBuilder.DropIndex(
                name: "IX_QuotationItems_ProductTypeVariantId",
                table: "QuotationItems");

            migrationBuilder.DropColumn(
                name: "ProductTypeVariantId",
                table: "QuotationItems");
        }
    }
}
