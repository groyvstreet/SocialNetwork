using FluentAssertions.Execution;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using System.Text;
using System.Security.Claims;
using PostService.Application.DTOs.CommentDTOs;

namespace PostServiceIntegrationTests.Controllers.CommentsControllerTests
{
    public class AddCommentAsyncTests : CommentsControllerTests
    {
        [Fact]
        public async Task AddCommentAsyncTestReturnsUnauthorized()
        {
            // Arrange
            var addCommentDTO = new AddCommentDTO();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/comments/");
            var body = JsonSerializer.Serialize(addCommentDTO);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AddCommentAsyncTestReturnsForbidden()
        {
            // Arrange
            var userId = _fakeUsersGenerator.Users.First().Id;
            var postId = _fakePostsGenerator.Posts.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addCommentDTO = new AddCommentDTO
            {
                Text = "text",
                UserId = userId,
                PostId = postId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/comments/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addCommentDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AddCommentAsyncTestReturnsBadRequest()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addCommentDTO = new AddCommentDTO();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/comments/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addCommentDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task AddCommentAsyncTestReturnsOK()
        {
            // Arrange
            var userId = _fakeUsersGenerator.Users.First().Id;
            var postId = _fakePostsGenerator.Posts.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addCommentDTO = new AddCommentDTO
            {
                Text = "text",
                UserId = userId,
                PostId = postId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/comments/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addCommentDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var commentJson = await response.Content.ReadAsStringAsync();
                var comment = JsonSerializer.Deserialize<GetCommentDTO>(commentJson, jsonSerializerOptions)!;
                comment.Text.Should().Be(addCommentDTO.Text);
                comment.UserId.Should().Be(addCommentDTO.UserId);
                comment.PostId.Should().Be(addCommentDTO.PostId);
                comment.LikeCount.Should().Be(0);
            }
        }
    }
}
