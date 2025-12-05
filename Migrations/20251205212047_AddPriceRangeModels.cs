using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrupoMad.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceRangeModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PriceRangesByDimensions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PriceListItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    MinWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxWidth = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinHeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxHeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceRangesByDimensions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceRangesByDimensions_PriceListItems_PriceListItemId",
                        column: x => x.PriceListItemId,
                        principalTable: "PriceListItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriceRangesByLength",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PriceListItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    MinLength = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxLength = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceRangesByLength", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceRangesByLength_PriceListItems_PriceListItemId",
                        column: x => x.PriceListItemId,
                        principalTable: "PriceListItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceRangesByDimensions_PriceListItemId",
                table: "PriceRangesByDimensions",
                column: "PriceListItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceRangesByLength_PriceListItemId",
                table: "PriceRangesByLength",
                column: "PriceListItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceRangesByDimensions");

            migrationBuilder.DropTable(
                name: "PriceRangesByLength");
        }
    }
}
