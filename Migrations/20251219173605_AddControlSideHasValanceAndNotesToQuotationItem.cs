using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrupoMad.Migrations
{
    /// <inheritdoc />
    public partial class AddControlSideHasValanceAndNotesToQuotationItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ControlSide",
                table: "QuotationItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasValance",
                table: "QuotationItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "QuotationItems",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ControlSide",
                table: "QuotationItems");

            migrationBuilder.DropColumn(
                name: "HasValance",
                table: "QuotationItems");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "QuotationItems");
        }
    }
}
