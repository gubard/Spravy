using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.ToDo.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddPeriodicity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoItem_ToDoItemGroupEntity_GroupId",
                table: "ToDoItem"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_ToDoItem_ToDoItemStatisticalEntity_StatisticalId",
                table: "ToDoItem"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_ToDoItem_ToDoItemValueEntity_ValueId",
                table: "ToDoItem"
            );

            /*migrationBuilder.DropTable(
                name: "ToDoItemGroupEntity"
            );

            migrationBuilder.DropTable(
                name: "ToDoItemStatisticalEntity"
            );

            migrationBuilder.DropTable(
                name: "ToDoItemValueEntity"
            );*/

            migrationBuilder.DropIndex(name: "IX_ToDoItem_GroupId", table: "ToDoItem");

            migrationBuilder.DropIndex(name: "IX_ToDoItem_StatisticalId", table: "ToDoItem");

            migrationBuilder.DropIndex(name: "IX_ToDoItem_ValueId", table: "ToDoItem");

            migrationBuilder.DropColumn(name: "GroupId", table: "ToDoItem");

            migrationBuilder.DropColumn(name: "StatisticalId", table: "ToDoItem");

            migrationBuilder.RenameColumn(name: "ValueId", table: "ToDoItem", newName: "DueDate");

            migrationBuilder.AddColumn<uint>(
                name: "CompletedCount",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u
            );

            migrationBuilder.AddColumn<uint>(
                name: "FailedCount",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u
            );

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<uint>(
                name: "SkippedCount",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u
            );

            migrationBuilder.AddColumn<byte>(
                name: "TypeOfPeriodicity",
                table: "ToDoItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "CompletedCount", table: "ToDoItem");

            migrationBuilder.DropColumn(name: "FailedCount", table: "ToDoItem");

            migrationBuilder.DropColumn(name: "IsCompleted", table: "ToDoItem");

            migrationBuilder.DropColumn(name: "SkippedCount", table: "ToDoItem");

            migrationBuilder.DropColumn(name: "TypeOfPeriodicity", table: "ToDoItem");

            migrationBuilder.RenameColumn(name: "DueDate", table: "ToDoItem", newName: "ValueId");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "ToDoItem",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.AddColumn<Guid>(
                name: "StatisticalId",
                table: "ToDoItem",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.CreateTable(
                name: "ToDoItemGroupEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoItemGroupEntity", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "ToDoItemStatisticalEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CompletedCount = table.Column<uint>(type: "INTEGER", nullable: false),
                    FailedCount = table.Column<uint>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SkippedCount = table.Column<uint>(type: "INTEGER", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoItemStatisticalEntity", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "ToDoItemValueEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    IsComplete = table.Column<bool>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TypeOfPeriodicity = table.Column<byte>(type: "INTEGER", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoItemValueEntity", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItem_GroupId",
                table: "ToDoItem",
                column: "GroupId",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItem_StatisticalId",
                table: "ToDoItem",
                column: "StatisticalId",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItem_ValueId",
                table: "ToDoItem",
                column: "ValueId",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoItem_ToDoItemGroupEntity_GroupId",
                table: "ToDoItem",
                column: "GroupId",
                principalTable: "ToDoItemGroupEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoItem_ToDoItemStatisticalEntity_StatisticalId",
                table: "ToDoItem",
                column: "StatisticalId",
                principalTable: "ToDoItemStatisticalEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoItem_ToDoItemValueEntity_ValueId",
                table: "ToDoItem",
                column: "ValueId",
                principalTable: "ToDoItemValueEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
