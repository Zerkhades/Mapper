using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mapper.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMapperDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeePhoto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePhoto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "geo_maps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImagePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ImageWidth = table.Column<int>(type: "integer", nullable: false),
                    ImageHeight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_geo_maps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "geo_marks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GeoMapId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    X = table.Column<double>(type: "double precision", nullable: false),
                    Y = table.Column<double>(type: "double precision", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CameraName = table.Column<string>(type: "text", nullable: true),
                    StreamUrl = table.Column<string>(type: "text", nullable: true),
                    TargetGeoMapId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkplaceCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_geo_marks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_geo_marks_geo_maps_GeoMapId",
                        column: x => x.GeoMapId,
                        principalTable: "geo_maps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    Patronymic = table.Column<string>(type: "text", nullable: true),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Cabinet = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    GeoMarkId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeePhotoId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_EmployeePhoto_EmployeePhotoId",
                        column: x => x.EmployeePhotoId,
                        principalTable: "EmployeePhoto",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_geo_marks_GeoMarkId",
                        column: x => x.GeoMarkId,
                        principalTable: "geo_marks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeePhotoId",
                table: "Employees",
                column: "EmployeePhotoId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_GeoMarkId",
                table: "Employees",
                column: "GeoMarkId");

            migrationBuilder.CreateIndex(
                name: "IX_geo_marks_GeoMapId",
                table: "geo_marks",
                column: "GeoMapId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "workplace_employees");

            migrationBuilder.DropTable(
                name: "EmployeePhoto");

            migrationBuilder.DropTable(
                name: "geo_marks");

            migrationBuilder.DropTable(
                name: "geo_maps");
        }
    }
}
