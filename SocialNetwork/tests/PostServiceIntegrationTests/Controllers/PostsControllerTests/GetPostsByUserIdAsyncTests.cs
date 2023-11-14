using FluentAssertions;
using FluentAssertions.Execution;
using PostService.Application.DTOs.PostDTOs;
using System.Net;
using System.Text.Json;

namespace PostServiceIntegrationTests.Controllers.PostsControllerTests
{
    public class GetPostsByUserIdAsyncTests : PostsControllerTests
    {
        [Fact]
        public async Task GetPostsByUserIdAsyncTestReturnsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/users/{userId}/posts");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetPostsByUserIdAsyncTestReturnsOK()
        {
            // Arrange
            var userId = _fakeUsersGenerator.Users.First().Id;

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/users/{userId}/posts");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var postJson = await response.Content.ReadAsStringAsync();
                var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var posts = JsonSerializer.Deserialize<List<GetPostDTO>>(postJson, jsonSerializerOptions)!;
                posts.Should().Contain(post => post.UserId == userId);
            }
        }
    }
}
