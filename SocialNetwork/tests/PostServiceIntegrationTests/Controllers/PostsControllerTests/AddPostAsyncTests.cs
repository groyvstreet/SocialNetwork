using FluentAssertions.Execution;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using System.Text;
using PostService.Application.DTOs.PostDTOs;
using System.Security.Claims;

namespace PostServiceIntegrationTests.Controllers.PostsControllerTests
{
    public class AddPostAsyncTests : PostsControllerTests
    {
        [Fact]
        public async Task AddPostAsyncTestReturnsUnauthorized()
        {
            // Arrange
            var addPostDTO = new AddPostDTO();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/posts/");
            var body = JsonSerializer.Serialize(addPostDTO);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AddPostAsyncTestReturnsForbidden()
        {
            // Arrange
            var userId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addPostDTO = new AddPostDTO
            {
                Text = "text",
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/posts/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addPostDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AddPostAsyncTestReturnsBadRequest()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addPostDTO = new AddPostDTO();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/posts/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addPostDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddPostAsyncTestReturnsOK()
        {
            // Arrange
            var userId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addPostDTO = new AddPostDTO
            {
                Text = "text",
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/posts/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addPostDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var postJson = await response.Content.ReadAsStringAsync();
                var post = JsonSerializer.Deserialize<GetPostDTO>(postJson, jsonSerializerOptions)!;
                post.Text.Should().Be(addPostDTO.Text);
                post.UserId.Should().Be(addPostDTO.UserId);
                post.CommentCount.Should().Be(0);
                post.LikeCount.Should().Be(0);
                post.RepostCount.Should().Be(0);
            }
        }
    }
}
