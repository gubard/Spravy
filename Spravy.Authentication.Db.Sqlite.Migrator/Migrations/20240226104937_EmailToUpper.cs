using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.Authentication.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class EmailToUpper : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE User SET Email = upper(Email) WHERE 1 > 0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
