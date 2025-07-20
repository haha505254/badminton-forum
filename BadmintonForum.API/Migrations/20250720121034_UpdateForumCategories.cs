using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BadmintonForum.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForumCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Icon", "Name" },
                values: new object[] { "羽毛球相關的一般討論", "💬", "綜合討論區" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Icon", "Name" },
                values: new object[] { "技術分享與教學討論", "🏸", "技術交流區" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Icon", "Name" },
                values: new object[] { "球拍、球鞋、裝備評測與推薦", "🎾", "裝備討論區" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Icon", "Name" },
                values: new object[] { "國內外賽事討論與轉播", "🏆", "賽事專區" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayOrder", "Icon", "Name" },
                values: new object[] { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "各地區球友交流與約球", 5, "📍", "地區球友會" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Icon", "Name" },
                values: new object[] { "分享和討論羽毛球技術", "🏸", "技術討論" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Icon", "Name" },
                values: new object[] { "球拍、球鞋等裝備討論", "🎾", "裝備推薦" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Icon", "Name" },
                values: new object[] { "比賽和活動信息", "📅", "活動公告" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Icon", "Name" },
                values: new object[] { "尋找球友，組織活動", "👥", "球友交流" });
        }
    }
}
