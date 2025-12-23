using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrupoMad.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressDetailsToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Companies",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Companies",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExteriorNumber",
                table: "Companies",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InteriorNumber",
                table: "Companies",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "Companies",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Companies",
                type: "TEXT",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RFC",
                table: "Companies",
                type: "TEXT",
                maxLength: 13,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StateID",
                table: "Companies",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Companies",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "City", "Email", "ExteriorNumber", "InteriorNumber", "Neighborhood", "PostalCode", "RFC", "StateID", "Street" },
                values: new object[] { null, null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "City", "Email", "ExteriorNumber", "InteriorNumber", "Neighborhood", "PostalCode", "RFC", "StateID", "Street" },
                values: new object[] { null, null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "City", "Email", "ExteriorNumber", "InteriorNumber", "Neighborhood", "PostalCode", "RFC", "StateID", "Street" },
                values: new object[] { null, null, null, null, null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ExteriorNumber",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "InteriorNumber",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "RFC",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "StateID",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Companies");
        }
    }
}
