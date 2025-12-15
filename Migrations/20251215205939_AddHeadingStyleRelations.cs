using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrupoMad.Migrations
{
    /// <inheritdoc />
    public partial class AddHeadingStyleRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceListItems_ProductTypeHeadingStyle_ProductTypeHeadingStyleId",
                table: "PriceListItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTypeHeadingStyle_ProductTypes_ProductTypeId",
                table: "ProductTypeHeadingStyle");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductTypeHeadingStyle",
                table: "ProductTypeHeadingStyle");

            migrationBuilder.RenameTable(
                name: "ProductTypeHeadingStyle",
                newName: "ProductTypeHeadingStyles");

            migrationBuilder.RenameIndex(
                name: "IX_ProductTypeHeadingStyle_ProductTypeId",
                table: "ProductTypeHeadingStyles",
                newName: "IX_ProductTypeHeadingStyles_ProductTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductTypeHeadingStyles",
                table: "ProductTypeHeadingStyles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceListItems_ProductTypeHeadingStyles_ProductTypeHeadingStyleId",
                table: "PriceListItems",
                column: "ProductTypeHeadingStyleId",
                principalTable: "ProductTypeHeadingStyles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTypeHeadingStyles_ProductTypes_ProductTypeId",
                table: "ProductTypeHeadingStyles",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceListItems_ProductTypeHeadingStyles_ProductTypeHeadingStyleId",
                table: "PriceListItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductTypeHeadingStyles_ProductTypes_ProductTypeId",
                table: "ProductTypeHeadingStyles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductTypeHeadingStyles",
                table: "ProductTypeHeadingStyles");

            migrationBuilder.RenameTable(
                name: "ProductTypeHeadingStyles",
                newName: "ProductTypeHeadingStyle");

            migrationBuilder.RenameIndex(
                name: "IX_ProductTypeHeadingStyles_ProductTypeId",
                table: "ProductTypeHeadingStyle",
                newName: "IX_ProductTypeHeadingStyle_ProductTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductTypeHeadingStyle",
                table: "ProductTypeHeadingStyle",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceListItems_ProductTypeHeadingStyle_ProductTypeHeadingStyleId",
                table: "PriceListItems",
                column: "ProductTypeHeadingStyleId",
                principalTable: "ProductTypeHeadingStyle",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductTypeHeadingStyle_ProductTypes_ProductTypeId",
                table: "ProductTypeHeadingStyle",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
