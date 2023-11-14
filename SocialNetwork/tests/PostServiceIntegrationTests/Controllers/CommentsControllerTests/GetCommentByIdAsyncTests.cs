using FluentAssertions;
using FluentAssertions.Execution;
using PostService.Application.DTOs.PostDTOs;
using System.Net;
using System.Text.Json;

namespace PostServiceIntegrationTests.Controllers.CommentsControllerTests
{
    public class GetCommentByIdAsyncTests : CommentsControllerTests
    {
        [Fact]
        public async Task GetCommentByIdAsyncTestReturnsNotFound()
        {
            // Arrange
            var commentId = Guid.NewGuid();

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/comments/{commentId}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetCommentByIdAsyncTestReturnsOK()
        {
            // Arrange
            var commentId = _fakeCommentsGenerator.Comments.First().Id;

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/comments/{commentId}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var commentJson = await response.Content.ReadAsStringAsync();
                var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var comment = JsonSerializer.Deserialize<GetPostDTO>(commentJson, jsonSerializerOptions)!;
                comment.Id.Should().Be(commentId);
            }
        }
    }
}
