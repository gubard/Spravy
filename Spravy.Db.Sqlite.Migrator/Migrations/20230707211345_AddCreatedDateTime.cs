using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDateTime",
                table: "ToDoItem",
                type: "TEXT",
                nullable: false,
                defaultValue: DateTimeOffset.Now
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "ToDoItem"
            );
        }
    }
}