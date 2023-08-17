using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.ToDo.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddToDoItemType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedCount",
                table: "ToDoItem");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "ToDoItem");

            migrationBuilder.DropColumn(
                name: "FailedCount",
                table: "ToDoItem");

            migrationBuilder.DropColumn(
                name: "IsComplete",
                table: "ToDoItem");

            migrationBuilder.DropColumn(
                name: "SkippedCount",
                table: "ToDoItem");

            migrationBuilder.RenameColumn(
                name: "TypeOfPeriodicity",
                table: "ToDoItem",
                newName: "Type");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "ToDoItem",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ValueId",
                table: "ToDoItem",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ToDoItemGroupEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoItemGroupEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ToDoItemValueEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsComplete = table.Column<bool>(type: "INTEGER", nullable: false),
                    TypeOfPeriodicity = table.Column<byte>(type: "INTEGER", nullable: false),
                    DueDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CompletedCount = table.Column<uint>(type: "INTEGER", nullable: false),
                    SkippedCount = table.Column<uint>(type: "INTEGER", nullable: false),
                    FailedCount = table.Column<uint>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoItemValueEntity", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItem_GroupId",
                table: "ToDoItem",
                column: "GroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItem_ValueId",
                table: "ToDoItem",
                column: "ValueId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoItem_ToDoItemGroupEntity_GroupId",
                table: "ToDoItem",
                column: "GroupId",
                principalTable: "ToDoItemGroupEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoItem_ToDoItemValueEntity_ValueId",
                table: "ToDoItem",
                column: "ValueId",
                principalTable: "ToDoItemValueEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoItem_ToDoItemGroupEntity_GroupId",
                table: "ToDoItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ToDoItem_ToDoItemValueEntity_ValueId",
                table: "ToDoItem");

            migrationBuilder.DropTable(
                name: "ToDoItemGroupEntity");

            migrationBuilder.DropTable(
                name: "ToDoItemValueEntity");

            migrationBuilder.DropIndex(
                name: "IX_ToDoItem_GroupId",
                table: "ToDoItem");

            migrationBuilder.DropIndex(
                name: "IX_ToDoItem_ValueId",
                table: "ToDoItem");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "ToDoItem");

            migrationBuilder.DropColumn(
                name: "ValueId",
                table: "ToDoItem");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "ToDoItem",
                newName: "TypeOfPeriodicity");

            migrationBuilder.AddColumn<uint>(
                name: "CompletedCount",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DueDate",
                table: "ToDoItem",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "FailedCount",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<bool>(
                name: "IsComplete",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<uint>(
                name: "SkippedCount",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);
        }
    }
}
