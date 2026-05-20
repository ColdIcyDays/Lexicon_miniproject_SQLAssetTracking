using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lexicon_Miniproject_SQLAssetTracking.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DBDevices",
                columns: table => new
                {
                    DBDevice_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceBrand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OfficeLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PurchaseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PurchasePrice_Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PurchasePrice_CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBDevices", x => x.DBDevice_Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DBDevices");
        }
    }
}
