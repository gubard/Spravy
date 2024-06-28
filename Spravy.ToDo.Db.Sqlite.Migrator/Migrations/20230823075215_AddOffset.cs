using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.ToDo.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddOffset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ushort>(
                name: "DaysOffset",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0
            );

            migrationBuilder.AddColumn<ushort>(
                name: "MonthsOffset",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0
            );

            migrationBuilder.AddColumn<ushort>(
                name: "WeeksOffset",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0
            );

            migrationBuilder.AddColumn<ushort>(
                name: "YearsOffset",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: (ushort)0
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "DaysOffset", table: "ToDoItem");

            migrationBuilder.DropColumn(name: "MonthsOffset", table: "ToDoItem");

            migrationBuilder.DropColumn(name: "WeeksOffset", table: "ToDoItem");

            migrationBuilder.DropColumn(name: "YearsOffset", table: "ToDoItem");
        }
    }
}
