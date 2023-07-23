using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddStatistical : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedCount",
                table: "ToDoItemValueEntity"
            );

            migrationBuilder.DropColumn(
                name: "FailedCount",
                table: "ToDoItemValueEntity"
            );

            migrationBuilder.DropColumn(
                name: "SkippedCount",
                table: "ToDoItemValueEntity"
            );

            migrationBuilder.AddColumn<Guid>(
                name: "StatisticalId",
                table: "ToDoItem",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.CreateTable(
                name: "ToDoItemStatisticalEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CompletedCount = table.Column<uint>(type: "INTEGER", nullable: false),
                    SkippedCount = table.Column<uint>(type: "INTEGER", nullable: false),
                    FailedCount = table.Column<uint>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_ToDoItemStatisticalEntity", x => x.Id); }
            );

            migrationBuilder.Sql(
                @$"INSERT INTO ToDoItemStatisticalEntity (Id, ItemId, CompletedCount, SkippedCount, FailedCount) 
            SELECT {SqliteSpravyDbContext.GenerateGuidQuery}, tdi.Id, 0, 0, 0
            FROM ToDoItem tdi;"
            );

            migrationBuilder.Sql("UPDATE ToDoItem AS tdi SET StatisticalId = (select tdse.Id FROM ToDoItemStatisticalEntity tdse WHERE tdi.Id = tdse.ItemId);");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItem_StatisticalId",
                table: "ToDoItem",
                column: "StatisticalId",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoItem_ToDoItemStatisticalEntity_StatisticalId",
                table: "ToDoItem",
                column: "StatisticalId",
                principalTable: "ToDoItemStatisticalEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoItem_ToDoItemStatisticalEntity_StatisticalId",
                table: "ToDoItem"
            );

            migrationBuilder.DropTable(
                name: "ToDoItemStatisticalEntity"
            );

            migrationBuilder.DropIndex(
                name: "IX_ToDoItem_StatisticalId",
                table: "ToDoItem"
            );

            migrationBuilder.DropColumn(
                name: "StatisticalId",
                table: "ToDoItem"
            );

            migrationBuilder.AddColumn<uint>(
                name: "CompletedCount",
                table: "ToDoItemValueEntity",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u
            );

            migrationBuilder.AddColumn<uint>(
                name: "FailedCount",
                table: "ToDoItemValueEntity",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u
            );

            migrationBuilder.AddColumn<uint>(
                name: "SkippedCount",
                table: "ToDoItemValueEntity",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u
            );
        }
    }
}