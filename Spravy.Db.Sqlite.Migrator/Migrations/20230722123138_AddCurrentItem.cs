using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCurrent",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCurrent",
                table: "ToDoItem");
        }
    }
}
