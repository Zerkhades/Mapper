using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mapper.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "workplace_employees");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "geo_marks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "geo_marks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "geo_maps",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "geo_maps",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "geo_marks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "geo_marks");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "geo_maps");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "geo_maps");

            migrationBuilder.CreateTable(
                name: "workplace_employees",
                columns: table => new
                {
                    WorkplaceMarkId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workplace_employees", x => new { x.WorkplaceMarkId, x.EmployeeId });
                    table.ForeignKey(
                        name: "FK_workplace_employees_geo_marks_WorkplaceMarkId",
                        column: x => x.WorkplaceMarkId,
                        principalTable: "geo_marks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
