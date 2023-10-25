using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PostEntityRepostCountFieldAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RepostCount",
                table: "Posts",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepostCount",
                table: "Posts");
        }
    }
}
