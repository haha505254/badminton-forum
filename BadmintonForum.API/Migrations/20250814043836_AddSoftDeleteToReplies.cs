using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BadmintonForum.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToReplies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Replies",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Replies",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Replies");
        }
    }
}
