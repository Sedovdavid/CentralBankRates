using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentralBankRates.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "currencies_catalog",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    eng_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    denomination = table.Column<int>(type: "int", nullable: false),
                    parent_code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    iso_num_code = table.Column<int>(type: "int", nullable: false),
                    iso_char_code = table.Column<string>(type: "varchar(4)", unicode: false, maxLength: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("currencies_catalog_pk", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rate_on_date",
                columns: table => new
                {
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    currencyId = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    rate = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("rate_on_date_pk", x => new { x.date, x.currencyId });
                });

            migrationBuilder.CreateIndex(
                name: "currencies_catalog_iso_char_code",
                table: "currencies_catalog",
                column: "iso_char_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "currencies_catalog");

            migrationBuilder.DropTable(
                name: "rate_on_date");
        }
    }
}
