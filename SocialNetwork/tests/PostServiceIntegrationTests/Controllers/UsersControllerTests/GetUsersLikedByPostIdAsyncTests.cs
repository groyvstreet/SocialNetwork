using FluentAssertions;
using System.Net;

namespace PostServiceIntegrationTests.Controllers.UsersControllerTests
{
    public class GetUsersLikedByPostIdAsyncTests : UsersControllerTests
    {
        [Fact]
        public async Task GetUsersLikedByPostIdAsyncTestReturnsNotFound()
        {
            // Arrange
            var postId = Guid.NewGuid();

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/posts/{postId}/likes/users");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetUsersLikedByPostIdAsyncTestReturnsOK()
        {
            // Arrange
            var postId = _fakePostsGenerator.Posts.First().Id;

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/posts/{postId}/likes/users");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
