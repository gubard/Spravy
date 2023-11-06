using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.ToDo.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToDoItemChildrenType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE ToDoItem SET ChildrenType = 1 WHERE ChildrenType = 2;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}