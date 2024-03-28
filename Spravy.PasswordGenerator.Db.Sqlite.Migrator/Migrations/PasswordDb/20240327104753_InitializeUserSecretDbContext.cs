using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.PasswordGenerator.Db.Sqlite.Migrator.Migrations.PasswordDb
{
    /// <inheritdoc />
    public partial class InitializeUserSecretDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    IsAvailableUpperLatin = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAvailableLowerLatin = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAvailableNumber = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAvailableSpecialSymbols = table.Column<bool>(type: "INTEGER", nullable: false),
                    CustomAvailableCharacters = table.Column<string>(type: "TEXT", nullable: false),
                    Length = table.Column<ushort>(type: "INTEGER", nullable: false),
                    Regex = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordItems_Key",
                table: "PasswordItems",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordItems_Name",
                table: "PasswordItems",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordItems");
        }
    }
}
