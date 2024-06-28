#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Spravy.PasswordGenerator.Db.Sqlite.Migrator.Migrations.PasswordDb;

/// <inheritdoc />
public partial class InitializeUserSecretDbContext : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "PasswordItems",
            table => new
            {
                Id = table.Column<Guid>("TEXT", nullable: false),
                Name = table.Column<string>("TEXT", nullable: false),
                Key = table.Column<string>("TEXT", nullable: false),
                IsAvailableUpperLatin = table.Column<bool>("INTEGER", nullable: false),
                IsAvailableLowerLatin = table.Column<bool>("INTEGER", nullable: false),
                IsAvailableNumber = table.Column<bool>("INTEGER", nullable: false),
                IsAvailableSpecialSymbols = table.Column<bool>("INTEGER", nullable: false),
                CustomAvailableCharacters = table.Column<string>("TEXT", nullable: false),
                Length = table.Column<ushort>("INTEGER", nullable: false),
                Regex = table.Column<string>("TEXT", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PasswordItems", x => x.Id);
            }
        );

        migrationBuilder.CreateIndex("IX_PasswordItems_Key", "PasswordItems", "Key", unique: true);
        migrationBuilder.CreateIndex(
            "IX_PasswordItems_Name",
            "PasswordItems",
            "Name",
            unique: true
        );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "PasswordItems");
    }
}
