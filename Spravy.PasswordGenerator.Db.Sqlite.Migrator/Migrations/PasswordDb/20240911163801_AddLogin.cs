#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Spravy.PasswordGenerator.Db.Sqlite.Migrator.Migrations.PasswordDb;

/// <inheritdoc />
public partial class AddLogin : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            "Login",
            "PasswordItems",
            "TEXT",
            nullable: false,
            defaultValue: ""
        );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn("Login", "PasswordItems");
    }
}