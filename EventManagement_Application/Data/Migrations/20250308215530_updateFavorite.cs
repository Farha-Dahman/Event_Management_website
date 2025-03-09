using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagement_Application.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateFavorite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Events_EventId",
                table: "Favorites");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Favorites",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventId1",
                table: "Favorites",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContactForms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactForms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_ApplicationUserId",
                table: "Favorites",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_EventId1",
                table: "Favorites",
                column: "EventId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Events_EventId",
                table: "Favorites",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Events_EventId1",
                table: "Favorites",
                column: "EventId1",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Users_ApplicationUserId",
                table: "Favorites",
                column: "ApplicationUserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Events_EventId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Events_EventId1",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Users_ApplicationUserId",
                table: "Favorites");

            migrationBuilder.DropTable(
                name: "ContactForms");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_ApplicationUserId",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_EventId1",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Favorites");

            migrationBuilder.DropColumn(
                name: "EventId1",
                table: "Favorites");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Events_EventId",
                table: "Favorites",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
