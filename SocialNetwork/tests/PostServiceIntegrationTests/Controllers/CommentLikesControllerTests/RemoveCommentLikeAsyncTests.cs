using FluentAssertions;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using PostService.Application.DTOs.CommentLikeDTOs;

namespace PostServiceIntegrationTests.Controllers.CommentLikesControllerTests
{
    public class RemoveCommentLikeAsyncTests : CommentLikesControllerTests
    {
        [Fact]
        public async Task RemoveCommentLikeAsyncTestReturnsUnauthorized()
        {
            // Arrange
            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO();

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/comment-likes/");
            var body = JsonSerializer.Serialize(addRemoveCommentLikeDTO);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task RemoveCommentLikeAsyncTestReturnsForbidden()
        {
            // Arrange
            var commentId = _fakeCommentsGenerator.Comments.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO()
            {
                CommentId = commentId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/comment-likes/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addRemoveCommentLikeDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task RemoveCommentLikeAsyncTestReturnsNotFound()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var userId = _fakeUsersGenerator.Users.First().Id;

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO()
            {
                CommentId = commentId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/comment-likes/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addRemoveCommentLikeDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task RemoveCommentLikeAsyncTestReturnsNoContent()
        {
            // Arrange
            var commentId = _fakeCommentsGenerator.Comments.First().Id;
            var userId = _fakeUsersGenerator.Users.Last().Id;

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO()
            {
                CommentId = commentId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/comment-likes/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addRemoveCommentLikeDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
