using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrupoMad.Migrations
{
    /// <inheritdoc />
    public partial class AddHeadingStyleToQuotationItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeadingStyle",
                table: "QuotationItems",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductTypeHeadingStyleId",
                table: "QuotationItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItems_ProductTypeHeadingStyleId",
                table: "QuotationItems",
                column: "ProductTypeHeadingStyleId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationItems_ProductTypeHeadingStyles_ProductTypeHeadingStyleId",
                table: "QuotationItems",
                column: "ProductTypeHeadingStyleId",
                principalTable: "ProductTypeHeadingStyles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationItems_ProductTypeHeadingStyles_ProductTypeHeadingStyleId",
                table: "QuotationItems");

            migrationBuilder.DropIndex(
                name: "IX_QuotationItems_ProductTypeHeadingStyleId",
                table: "QuotationItems");

            migrationBuilder.DropColumn(
                name: "HeadingStyle",
                table: "QuotationItems");

            migrationBuilder.DropColumn(
                name: "ProductTypeHeadingStyleId",
                table: "QuotationItems");
        }
    }
}
