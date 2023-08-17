using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.ToDo.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ToDoItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TypeOfPeriodicity = table.Column<byte>(type: "INTEGER", nullable: false),
                    DueDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    IsComplete = table.Column<bool>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToDoItem_ToDoItem_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ToDoItem",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItem_ParentId",
                table: "ToDoItem",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToDoItem");
        }
    }
}
