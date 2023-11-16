using FluentAssertions;
using System.Net;

namespace PostServiceIntegrationTests.Controllers.UsersControllerTests
{
    public class GetUsersLikedByCommentIdAsync : UsersControllerTests
    {
        [Fact]
        public async Task GetUsersLikedByCommentIdAsyncTestReturnsNotFound()
        {
            // Arrange
            var commentId = Guid.NewGuid();

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/comments/{commentId}/likes/users");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetUsersLikedByCommentIdAsyncTestReturnsOK()
        {
            // Arrange
            var commentId = _fakeCommentsGenerator.Comments.First().Id;

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/comments/{commentId}/likes/users");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
