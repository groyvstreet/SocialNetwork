using FluentAssertions;
using FluentAssertions.Execution;
using PostService.Application.DTOs.PostDTOs;
using System.Net;
using System.Text.Json;

namespace PostServiceIntegrationTests.Controllers.PostsControllerTests
{
    public class GetPostByIdAsyncTests : PostsControllerTests
    {
        [Fact]
        public async Task GetPostByIdAsyncTestReturnsNotFound()
        {
            // Arrange
            var postId = Guid.NewGuid();

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/posts/{postId}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetPostByIdAsyncTestReturnsOK()
        {
            // Arrange
            var postId = _fakePostsGenerator.Posts.First().Id;

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/posts/{postId}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var postJson = await response.Content.ReadAsStringAsync();
                var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var post = JsonSerializer.Deserialize<GetPostDTO>(postJson, jsonSerializerOptions)!;
                post.Id.Should().Be(postId);
            }
        }
    }
}
