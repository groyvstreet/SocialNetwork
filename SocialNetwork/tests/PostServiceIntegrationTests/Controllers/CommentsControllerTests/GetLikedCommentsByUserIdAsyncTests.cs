using FluentAssertions;
using FluentAssertions.Execution;
using PostService.Application.DTOs.CommentDTOs;
using System.Net;
using System.Text.Json;

namespace PostServiceIntegrationTests.Controllers.CommentsControllerTests
{
    public class GetLikedCommentsByUserIdAsyncTests : CommentsControllerTests
    {
        [Fact]
        public async Task GetLikedCommentsByUserIdAsyncTestReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/users/{userId}/comment-likes/comments");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetLikedCommentsByUserIdAsyncTestReturnsOK()
        {
            // Arrange
            var userId = _fakeUsersGenerator.Users.First().Id;

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/users/{userId}/comment-likes/comments");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var commentsJson = await response.Content.ReadAsStringAsync();
                var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var comments = JsonSerializer.Deserialize<List<GetCommentDTO>>(commentsJson, jsonSerializerOptions)!;
                comments.Should().Contain(comment => comment.UserId == userId);
            }
        }
    }
}
