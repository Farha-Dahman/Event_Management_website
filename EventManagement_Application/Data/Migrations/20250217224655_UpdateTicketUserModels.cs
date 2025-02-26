using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagement_Application.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTicketUserModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketUser_Tickets_TicketId",
                table: "TicketUser");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketUser_Users_UserId",
                table: "TicketUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketUser",
                table: "TicketUser");

            migrationBuilder.RenameTable(
                name: "TicketUser",
                newName: "TicketUsers");

            migrationBuilder.RenameIndex(
                name: "IX_TicketUser_UserId",
                table: "TicketUsers",
                newName: "IX_TicketUsers_UserId");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "TicketUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketUsers",
                table: "TicketUsers",
                columns: new[] { "TicketId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TicketUsers_Tickets_TicketId",
                table: "TicketUsers",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketUsers_Users_UserId",
                table: "TicketUsers",
                column: "UserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketUsers_Tickets_TicketId",
                table: "TicketUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketUsers_Users_UserId",
                table: "TicketUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketUsers",
                table: "TicketUsers");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "TicketUsers");

            migrationBuilder.RenameTable(
                name: "TicketUsers",
                newName: "TicketUser");

            migrationBuilder.RenameIndex(
                name: "IX_TicketUsers_UserId",
                table: "TicketUser",
                newName: "IX_TicketUser_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketUser",
                table: "TicketUser",
                columns: new[] { "TicketId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TicketUser_Tickets_TicketId",
                table: "TicketUser",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketUser_Users_UserId",
                table: "TicketUser",
                column: "UserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
