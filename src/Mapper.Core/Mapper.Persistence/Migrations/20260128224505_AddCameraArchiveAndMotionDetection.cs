using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mapper.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCameraArchiveAndMotionDetection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "camera_motion_alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CameraMarkId = table.Column<Guid>(type: "uuid", nullable: false),
                    DetectedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ConfirmedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    MotionPercentage = table.Column<double>(type: "double precision", nullable: false),
                    SnapshotPath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false),
                    ResolutionNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RelatedVideoArchiveId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_camera_motion_alerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "camera_status_histories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CameraMarkId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsOnline = table.Column<bool>(type: "boolean", nullable: false),
                    Reason = table.Column<int>(type: "integer", nullable: false),
                    ChangedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DurationSinceLastChange = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Details = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ResponseTimeMs = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_camera_status_histories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "camera_video_archives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CameraMarkId = table.Column<Guid>(type: "uuid", nullable: false),
                    VideoPath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ThumbnailPath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RecordedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    HasMotionDetected = table.Column<bool>(type: "boolean", nullable: false),
                    Resolution = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FramesPerSecond = table.Column<int>(type: "integer", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_camera_video_archives", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_camera_motion_alerts_CameraMarkId",
                table: "camera_motion_alerts",
                column: "CameraMarkId");

            migrationBuilder.CreateIndex(
                name: "IX_camera_motion_alerts_DetectedAt",
                table: "camera_motion_alerts",
                column: "DetectedAt");

            migrationBuilder.CreateIndex(
                name: "IX_camera_motion_alerts_IsResolved",
                table: "camera_motion_alerts",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_camera_status_histories_CameraMarkId",
                table: "camera_status_histories",
                column: "CameraMarkId");

            migrationBuilder.CreateIndex(
                name: "IX_camera_status_histories_ChangedAt",
                table: "camera_status_histories",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_camera_video_archives_CameraMarkId",
                table: "camera_video_archives",
                column: "CameraMarkId");

            migrationBuilder.CreateIndex(
                name: "IX_camera_video_archives_RecordedAt",
                table: "camera_video_archives",
                column: "RecordedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "camera_motion_alerts");

            migrationBuilder.DropTable(
                name: "camera_status_histories");

            migrationBuilder.DropTable(
                name: "camera_video_archives");
        }
    }
}
