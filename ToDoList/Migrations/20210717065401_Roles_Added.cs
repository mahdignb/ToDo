using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoList.Migrations
{
    public partial class Roles_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3d8b9cd0-1df4-4cf7-a80d-1cd3917f77ef", "a31840e1-491b-4858-962b-33eb8d0e0de5", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8f44a26b-c3ee-4c40-93ed-0ebd456b82ee", "939633b5-bc20-4eb2-9005-7b3cc0115d15", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d8b9cd0-1df4-4cf7-a80d-1cd3917f77ef");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8f44a26b-c3ee-4c40-93ed-0ebd456b82ee");
        }
    }
}
