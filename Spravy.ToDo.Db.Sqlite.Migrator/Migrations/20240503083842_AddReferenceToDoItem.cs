using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.ToDo.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddReferenceToDoItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DependencyToDoItem");

            migrationBuilder.AddColumn<Guid>(
                name: "ReferenceId",
                table: "ToDoItem",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItem_ReferenceId",
                table: "ToDoItem",
                column: "ReferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoItem_ToDoItem_ReferenceId",
                table: "ToDoItem",
                column: "ReferenceId",
                principalTable: "ToDoItem",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoItem_ToDoItem_ReferenceId",
                table: "ToDoItem");

            migrationBuilder.DropIndex(
                name: "IX_ToDoItem_ReferenceId",
                table: "ToDoItem");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "ToDoItem");

            migrationBuilder.CreateTable(
                name: "DependencyToDoItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DependencyToDoItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ToDoItemId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DependencyToDoItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DependencyToDoItem_ToDoItem_DependencyToDoItemId",
                        column: x => x.DependencyToDoItemId,
                        principalTable: "ToDoItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DependencyToDoItem_ToDoItem_ToDoItemId",
                        column: x => x.ToDoItemId,
                        principalTable: "ToDoItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DependencyToDoItem_DependencyToDoItemId",
                table: "DependencyToDoItem",
                column: "DependencyToDoItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DependencyToDoItem_ToDoItemId",
                table: "DependencyToDoItem",
                column: "ToDoItemId");
        }
    }
}
