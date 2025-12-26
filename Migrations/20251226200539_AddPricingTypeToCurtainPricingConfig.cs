using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrupoMad.Migrations
{
    /// <inheritdoc />
    public partial class AddPricingTypeToCurtainPricingConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PricingType",
                table: "CurtainPricingConfigs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricingType",
                table: "CurtainPricingConfigs");
        }
    }
}
