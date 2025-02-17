using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventManagement_Application.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Events",
                newName: "EventDate");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Events",
                newName: "CreationDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventDate",
                table: "Events",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "CreationDate",
                table: "Events",
                newName: "EndDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Events",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
