using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.PasswordGenerator.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class InitializeUserSecretDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSecrets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Secret = table.Column<byte[]>(type: "BLOB", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecrets", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserSecrets_UserId",
                table: "UserSecrets",
                column: "UserId",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UserSecrets");
        }
    }
}
