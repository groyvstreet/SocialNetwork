using FluentAssertions;
using FluentAssertions.Execution;
using PostService.Application.DTOs.CommentDTOs;
using System.Net;
using System.Text.Json;

namespace PostServiceIntegrationTests.Controllers.CommentsControllerTests
{
    public class GetCommentsByPostIdAsyncTests : CommentsControllerTests
    {
        [Fact]
        public async Task GetCommentsByPostIdAsyncTestReturnsNotFound()
        {
            // Arrange
            var postId = Guid.NewGuid();

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/posts/{postId}/comments");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetCommentsByPostIdAsyncTestReturnsOK()
        {
            // Arrange
            var postId = _fakePostsGenerator.Posts.First().Id;

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/posts/{postId}/comments");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var commentsJson = await response.Content.ReadAsStringAsync();
                var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var comments = JsonSerializer.Deserialize<List<GetCommentDTO>>(commentsJson, jsonSerializerOptions)!;
                comments.Should().Contain(comment => comment.PostId == postId);
            }
        }
    }
}
