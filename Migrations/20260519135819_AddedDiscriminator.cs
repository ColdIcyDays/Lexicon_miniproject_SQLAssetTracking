using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lexicon_Miniproject_SQLAssetTracking.Migrations
{
    /// <inheritdoc />
    public partial class AddedDiscriminator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceType",
                table: "DBDevices",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "DBDevices");
        }
    }
}
