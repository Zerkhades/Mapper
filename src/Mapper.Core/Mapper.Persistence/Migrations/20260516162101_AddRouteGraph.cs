using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mapper.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRouteGraph : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "route_nodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GeoMapId = table.Column<Guid>(type: "uuid", nullable: false),
                    GeoMarkId = table.Column<Guid>(type: "uuid", nullable: true),
                    X = table.Column<double>(type: "double precision", nullable: false),
                    Y = table.Column<double>(type: "double precision", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_route_nodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_route_nodes_geo_maps_GeoMapId",
                        column: x => x.GeoMapId,
                        principalTable: "geo_maps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_route_nodes_geo_marks_GeoMarkId",
                        column: x => x.GeoMarkId,
                        principalTable: "geo_marks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "route_edges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GeoMapId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromNodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToNodeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CostOverride = table.Column<double>(type: "double precision", nullable: true),
                    IsBidirectional = table.Column<bool>(type: "boolean", nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_route_edges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_route_edges_geo_maps_GeoMapId",
                        column: x => x.GeoMapId,
                        principalTable: "geo_maps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_route_edges_route_nodes_FromNodeId",
                        column: x => x.FromNodeId,
                        principalTable: "route_nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_route_edges_route_nodes_ToNodeId",
                        column: x => x.ToNodeId,
                        principalTable: "route_nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_route_edges_FromNodeId",
                table: "route_edges",
                column: "FromNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_route_edges_GeoMapId",
                table: "route_edges",
                column: "GeoMapId");

            migrationBuilder.CreateIndex(
                name: "IX_route_edges_ToNodeId",
                table: "route_edges",
                column: "ToNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_route_nodes_GeoMapId",
                table: "route_nodes",
                column: "GeoMapId");

            migrationBuilder.CreateIndex(
                name: "IX_route_nodes_GeoMarkId",
                table: "route_nodes",
                column: "GeoMarkId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "route_edges");

            migrationBuilder.DropTable(
                name: "route_nodes");
        }
    }
}
