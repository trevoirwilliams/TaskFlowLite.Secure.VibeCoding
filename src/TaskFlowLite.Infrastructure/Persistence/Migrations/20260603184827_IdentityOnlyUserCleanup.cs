using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlowLite.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IdentityOnlyUserCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUsers_Users_DomainUserId",
                table: "AppUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestNotes_Users_AuthorUserId",
                table: "RequestNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkRequests_Users_AssignedToUserId",
                table: "WorkRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkRequests_Users_RequestedByUserId",
                table: "WorkRequests");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "AppUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AppUsers",
                type: "TEXT",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AppUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);

            migrationBuilder.Sql(
                """
                UPDATE AppUsers
                SET DisplayName = COALESCE((SELECT DisplayName FROM Users WHERE Users.Id = AppUsers.DomainUserId), ''),
                    IsActive = COALESCE((SELECT IsActive FROM Users WHERE Users.Id = AppUsers.DomainUserId), 1),
                    CreatedAtUtc = COALESCE((SELECT CreatedAtUtc FROM Users WHERE Users.Id = AppUsers.DomainUserId), CreatedAtUtc);
                """);

            migrationBuilder.Sql(
                """
                UPDATE WorkRequests
                SET RequestedByUserId = COALESCE((SELECT Id FROM AppUsers WHERE AppUsers.DomainUserId = WorkRequests.RequestedByUserId), RequestedByUserId),
                    AssignedToUserId = (SELECT Id FROM AppUsers WHERE AppUsers.DomainUserId = WorkRequests.AssignedToUserId)
                ;
                """);

            migrationBuilder.Sql(
                """
                UPDATE RequestNotes
                SET AuthorUserId = COALESCE((SELECT Id FROM AppUsers WHERE AppUsers.DomainUserId = RequestNotes.AuthorUserId), AuthorUserId);
                """);

            migrationBuilder.DropIndex(
                name: "IX_AppUsers_DomainUserId",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "DomainUserId",
                table: "AppUsers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestNotes_AppUsers_AuthorUserId",
                table: "RequestNotes",
                column: "AuthorUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkRequests_AppUsers_AssignedToUserId",
                table: "WorkRequests",
                column: "AssignedToUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkRequests_AppUsers_RequestedByUserId",
                table: "WorkRequests",
                column: "RequestedByUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestNotes_AppUsers_AuthorUserId",
                table: "RequestNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkRequests_AppUsers_AssignedToUserId",
                table: "WorkRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkRequests_AppUsers_RequestedByUserId",
                table: "WorkRequests");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AppUsers");

            migrationBuilder.AddColumn<int>(
                name: "DomainUserId",
                table: "AppUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.Sql(
                """
                INSERT INTO Users (Id, DisplayName, Email, IsActive, CreatedAtUtc)
                SELECT Id,
                       COALESCE(DisplayName, ''),
                       COALESCE(Email, 'restored-user-' || Id || '@taskflow.local'),
                       IsActive,
                       CreatedAtUtc
                FROM AppUsers;

                UPDATE AppUsers
                SET DomainUserId = Id;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_DomainUserId",
                table: "AppUsers",
                column: "DomainUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUsers_Users_DomainUserId",
                table: "AppUsers",
                column: "DomainUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestNotes_Users_AuthorUserId",
                table: "RequestNotes",
                column: "AuthorUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkRequests_Users_AssignedToUserId",
                table: "WorkRequests",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkRequests_Users_RequestedByUserId",
                table: "WorkRequests",
                column: "RequestedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
