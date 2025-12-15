using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrupoMad.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTypeHeadingStyles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasHeadingStyles",
                table: "ProductTypes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProductTypeHeadingStyleId",
                table: "PriceListItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductTypeHeadingStyle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypeHeadingStyle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductTypeHeadingStyle_ProductTypes_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceListItems_ProductTypeHeadingStyleId",
                table: "PriceListItems",
                column: "ProductTypeHeadingStyleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTypeHeadingStyle_ProductTypeId",
                table: "ProductTypeHeadingStyle",
                column: "ProductTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceListItems_ProductTypeHeadingStyle_ProductTypeHeadingStyleId",
                table: "PriceListItems",
                column: "ProductTypeHeadingStyleId",
                principalTable: "ProductTypeHeadingStyle",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceListItems_ProductTypeHeadingStyle_ProductTypeHeadingStyleId",
                table: "PriceListItems");

            migrationBuilder.DropTable(
                name: "ProductTypeHeadingStyle");

            migrationBuilder.DropIndex(
                name: "IX_PriceListItems_ProductTypeHeadingStyleId",
                table: "PriceListItems");

            migrationBuilder.DropColumn(
                name: "HasHeadingStyles",
                table: "ProductTypes");

            migrationBuilder.DropColumn(
                name: "ProductTypeHeadingStyleId",
                table: "PriceListItems");
        }
    }
}
