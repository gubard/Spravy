using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.PasswordGenerator.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class InitializePasswordDbContext : Migration
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
                    AvailableCharacters = table.Column<string>(type: "TEXT", nullable: false),
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
