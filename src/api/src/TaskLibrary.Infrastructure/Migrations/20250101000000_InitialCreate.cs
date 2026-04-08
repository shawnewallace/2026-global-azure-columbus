using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskLibrary.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "tasks",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                description = table.Column<string>(type: "text", nullable: true),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Backlog"),
                priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Medium"),
                category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                ai_suggested_priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                ai_suggested_category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                ai_reasoning = table.Column<string>(type: "text", nullable: true),
                ai_suggestion_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_tasks", x => x.id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "tasks");
    }
}
