using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrupoMad.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTypeIdToPriceList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductTypeId",
                table: "PriceLists",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceLists_ProductTypeId",
                table: "PriceLists",
                column: "ProductTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceLists_ProductTypes_ProductTypeId",
                table: "PriceLists",
                column: "ProductTypeId",
                principalTable: "ProductTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceLists_ProductTypes_ProductTypeId",
                table: "PriceLists");

            migrationBuilder.DropIndex(
                name: "IX_PriceLists_ProductTypeId",
                table: "PriceLists");

            migrationBuilder.DropColumn(
                name: "ProductTypeId",
                table: "PriceLists");
        }
    }
}
