using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.ToDo.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddDaysOf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DaysOfMonth",
                table: "ToDoItem",
                type: "TEXT",
                nullable: false,
                defaultValue: "1");

            migrationBuilder.AddColumn<string>(
                name: "DaysOfWeek",
                table: "ToDoItem",
                type: "TEXT",
                nullable: false,
                defaultValue: "Monday");

            migrationBuilder.AddColumn<string>(
                name: "DaysOfYear",
                table: "ToDoItem",
                type: "TEXT",
                nullable: false,
                defaultValue: "1.1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaysOfMonth",
                table: "ToDoItem");

            migrationBuilder.DropColumn(
                name: "DaysOfWeek",
                table: "ToDoItem");

            migrationBuilder.DropColumn(
                name: "DaysOfYear",
                table: "ToDoItem");
        }
    }
}
