using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.PasswordGenerator.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddIs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvailableCharacters",
                table: "PasswordItems",
                newName: "CustomAvailableCharacters");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailableLowerLatin",
                table: "PasswordItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailableNumber",
                table: "PasswordItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailableSpecialSymbols",
                table: "PasswordItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailableUpperLatin",
                table: "PasswordItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailableLowerLatin",
                table: "PasswordItems");

            migrationBuilder.DropColumn(
                name: "IsAvailableNumber",
                table: "PasswordItems");

            migrationBuilder.DropColumn(
                name: "IsAvailableSpecialSymbols",
                table: "PasswordItems");

            migrationBuilder.DropColumn(
                name: "IsAvailableUpperLatin",
                table: "PasswordItems");

            migrationBuilder.RenameColumn(
                name: "CustomAvailableCharacters",
                table: "PasswordItems",
                newName: "AvailableCharacters");
        }
    }
}
