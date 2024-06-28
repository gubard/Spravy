using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.EventBus.Db.Sqlite.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class AddPushedDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PushedDateTime",
                table: "Event",
                type: "TEXT",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "PushedDateTime", table: "Event");
        }
    }
}
