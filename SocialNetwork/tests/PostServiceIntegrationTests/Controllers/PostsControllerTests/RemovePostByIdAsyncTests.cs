using FluentAssertions;
using System.Net;
using System.Security.Claims;

namespace PostServiceIntegrationTests.Controllers.PostsControllerTests
{
    public class RemovePostByIdAsyncTests : PostsControllerTests
    {
        [Fact]
        public async Task RemovePostByIdAsyncTestReturnsUnauthorized()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/posts/{Guid.NewGuid()}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task RemovePostByIdAsyncTestReturnsForbidden()
        {
            // Arrange
            var postId = _fakePostsGenerator.Posts.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/posts/{postId}");
            request.Headers.Add("Authorization", $"Bearer {token}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task RemovePostByIdAsyncTestReturnsNotFound()
        {
            // Arrange
            var postId = _fakePostsGenerator.Posts.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/posts/{Guid.NewGuid()}");
            request.Headers.Add("Authorization", $"Bearer {token}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task RemovePostByIdAsyncTestReturnsNoContent()
        {
            // Arrange
            var postId = _fakePostsGenerator.Posts.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/posts/{postId}");
            request.Headers.Add("Authorization", $"Bearer {token}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
