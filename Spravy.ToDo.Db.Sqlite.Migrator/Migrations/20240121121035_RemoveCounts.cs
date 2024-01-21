using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.ToDo.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedCount",
                table: "ToDoItem");

            migrationBuilder.DropColumn(
                name: "FailedCount",
                table: "ToDoItem");

            migrationBuilder.DropColumn(
                name: "LastCompletedType",
                table: "ToDoItem");

            migrationBuilder.DropColumn(
                name: "SkippedCount",
                table: "ToDoItem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "CompletedCount",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "FailedCount",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<int>(
                name: "LastCompletedType",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "SkippedCount",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);
        }
    }
}
