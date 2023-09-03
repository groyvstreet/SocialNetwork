using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostService.API.Migrations
{
    /// <inheritdoc />
    public partial class RenamedEntityUserProfileToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentLikes_UserProfiles_UserProfileId",
                table: "CommentLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_UserProfiles_UserProfileId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLikes_UserProfiles_UserProfileId",
                table: "PostLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_UserProfiles_UserProfileId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                table: "Posts",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UserProfileId",
                table: "Posts",
                newName: "IX_Posts_UserId");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                table: "PostLikes",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_PostLikes_UserProfileId",
                table: "PostLikes",
                newName: "IX_PostLikes_UserId");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                table: "Comments",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserProfileId",
                table: "Comments",
                newName: "IX_Comments_UserId");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                table: "CommentLikes",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentLikes_UserProfileId",
                table: "CommentLikes",
                newName: "IX_CommentLikes_UserId");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "FirstName", "Image", "LastName" },
                values: new object[] { new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), new DateOnly(1, 1, 1), "", "", "" });

            migrationBuilder.AddForeignKey(
                name: "FK_CommentLikes_Users_UserId",
                table: "CommentLikes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikes_Users_UserId",
                table: "PostLikes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_UserId",
                table: "Posts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentLikes_Users_UserId",
                table: "CommentLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_UserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLikes_Users_UserId",
                table: "PostLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_UserId",
                table: "Posts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Posts",
                newName: "UserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                newName: "IX_Posts_UserProfileId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "PostLikes",
                newName: "UserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_PostLikes_UserId",
                table: "PostLikes",
                newName: "IX_PostLikes_UserProfileId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Comments",
                newName: "UserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                newName: "IX_Comments_UserProfileId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CommentLikes",
                newName: "UserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_CommentLikes_UserId",
                table: "CommentLikes",
                newName: "IX_CommentLikes_UserProfileId");

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "BirthDate", "FirstName", "Image", "LastName" },
                values: new object[] { new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), new DateOnly(1, 1, 1), "", "", "" });

            migrationBuilder.AddForeignKey(
                name: "FK_CommentLikes_UserProfiles_UserProfileId",
                table: "CommentLikes",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_UserProfiles_UserProfileId",
                table: "Comments",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikes_UserProfiles_UserProfileId",
                table: "PostLikes",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_UserProfiles_UserProfileId",
                table: "Posts",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
