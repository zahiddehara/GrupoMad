using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrupoMad.Migrations
{
    /// <inheritdoc />
    public partial class AddProductColorToQuotationItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductColorId",
                table: "QuotationItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItems_ProductColorId",
                table: "QuotationItems",
                column: "ProductColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationItems_ProductColors_ProductColorId",
                table: "QuotationItems",
                column: "ProductColorId",
                principalTable: "ProductColors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationItems_ProductColors_ProductColorId",
                table: "QuotationItems");

            migrationBuilder.DropIndex(
                name: "IX_QuotationItems_ProductColorId",
                table: "QuotationItems");

            migrationBuilder.DropColumn(
                name: "ProductColorId",
                table: "QuotationItems");
        }
    }
}
