using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostService.API.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "BirthDate", "FirstName", "Image", "LastName" },
                values: new object[] { new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), new DateOnly(1, 1, 1), "", "", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"));
        }
    }
}
