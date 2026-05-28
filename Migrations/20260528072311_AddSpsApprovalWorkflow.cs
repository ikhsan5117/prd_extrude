using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelastoProductionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSpsApprovalWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    TargetKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RequestComment = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ReturnUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    RequesterUserName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RequesterRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApproverUserName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ApproverRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApproverComment = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ApprovalToken = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    TokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConsumedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalRequestLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApprovalRequestId = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: true),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ActorUserName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ActorRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRequestLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalRequestLogs_ApprovalRequests_ApprovalRequestId",
                        column: x => x.ApprovalRequestId,
                        principalTable: "ApprovalRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequestLogs_ApprovalRequestId",
                table: "ApprovalRequestLogs",
                column: "ApprovalRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_ActionType_TargetKey_RequesterUserName_Status",
                table: "ApprovalRequests",
                columns: new[] { "ActionType", "TargetKey", "RequesterUserName", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_RequestCode",
                table: "ApprovalRequests",
                column: "RequestCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_RequesterUserName_Status",
                table: "ApprovalRequests",
                columns: new[] { "RequesterUserName", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_Status_CreatedAt",
                table: "ApprovalRequests",
                columns: new[] { "Status", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalRequestLogs");

            migrationBuilder.DropTable(
                name: "ApprovalRequests");
        }
    }
}
