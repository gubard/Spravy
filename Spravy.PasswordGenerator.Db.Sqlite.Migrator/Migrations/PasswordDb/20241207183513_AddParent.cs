using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spravy.PasswordGenerator.Db.Sqlite.Migrator.Migrations.PasswordDb
{
    /// <inheritdoc />
    public partial class AddParent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "OrderIndex",
                table: "PasswordItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "PasswordItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "PasswordItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordItems_ParentId",
                table: "PasswordItems",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordItems_PasswordItems_ParentId",
                table: "PasswordItems",
                column: "ParentId",
                principalTable: "PasswordItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PasswordItems_PasswordItems_ParentId",
                table: "PasswordItems");

            migrationBuilder.DropIndex(
                name: "IX_PasswordItems_ParentId",
                table: "PasswordItems");

            migrationBuilder.DropColumn(
                name: "OrderIndex",
                table: "PasswordItems");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "PasswordItems");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "PasswordItems");
        }
    }
}
