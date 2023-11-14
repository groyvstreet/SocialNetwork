using FluentAssertions;
using FluentAssertions.Execution;
using PostService.Application.DTOs.CommentLikeDTOs;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace PostServiceIntegrationTests.Controllers.CommentLikesControllerTests
{
    public class AddCommentLikeAsyncTests : CommentLikesControllerTests
    {
        [Fact]
        public async Task AddCommentLikeAsyncTestReturnsUnauthorized()
        {
            // Arrange
            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/comment-likes/");
            var body = JsonSerializer.Serialize(addRemoveCommentLikeDTO);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AddCommentLikeAsyncTestReturnsForbidden()
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

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/comment-likes/");
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
        public async Task AddCommentLikeAsyncTestReturnsNotFound()
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

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/comment-likes/");
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
        public async Task AddCommentLikeAsyncTestReturnsConflict()
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

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/comment-likes/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addRemoveCommentLikeDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task AddCommentLikeAsyncTestReturnsOK()
        {
            // Arrange
            var commentId = _fakeCommentsGenerator.Comments.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addRemoveCommentLikeDTO = new AddRemoveCommentLikeDTO()
            {
                CommentId = commentId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/comment-likes/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addRemoveCommentLikeDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var commentLikeJson = await response.Content.ReadAsStringAsync();
                var commentLike = JsonSerializer.Deserialize<GetCommentLikeDTO>(commentLikeJson, jsonSerializerOptions)!;
                commentLike.UserId.Should().Be(addRemoveCommentLikeDTO.UserId);
                commentLike.CommentId.Should().Be(addRemoveCommentLikeDTO.CommentId);
            }
        }
    }
}
