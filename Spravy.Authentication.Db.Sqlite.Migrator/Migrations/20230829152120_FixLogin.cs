using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.Authentication.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class FixLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_User_Login",
                table: "User",
                column: "Login",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_User_Login", table: "User");
        }
    }
}
