using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.ToDo.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "OrderIndex",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderIndex",
                table: "ToDoItem");
        }
    }
}
