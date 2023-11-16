using FluentAssertions;
using FluentAssertions.Execution;
using PostService.Application.DTOs.PostLikeDTOs;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace PostServiceIntegrationTests.Controllers.PostLikesControllerTests
{
    public class AddPostLikeAsyncTests : PostLikesControllerTests
    {
        [Fact]
        public async Task AddPostLikeAsyncTestReturnsUnauthorized()
        {
            // Arrange
            var addRemovePostLikeDTO = new AddRemovePostLikeDTO();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/post-likes/");
            var body = JsonSerializer.Serialize(addRemovePostLikeDTO);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            
            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AddPostLikeAsyncTestReturnsForbidden()
        {
            // Arrange
            var postId = _fakePostsGenerator.Posts.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addRemovePostLikeDTO = new AddRemovePostLikeDTO()
            {
                PostId = postId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/post-likes/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addRemovePostLikeDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AddPostLikeAsyncTestReturnsNotFound()
        {
            // Arrange
            var postId = Guid.NewGuid();
            var userId = _fakeUsersGenerator.Users.First().Id;

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addRemovePostLikeDTO = new AddRemovePostLikeDTO()
            {
                PostId = postId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/post-likes/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addRemovePostLikeDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddPostLikeAsyncTestReturnsConflict()
        {
            // Arrange
            var postId = _fakePostsGenerator.Posts.First().Id;
            var userId = _fakeUsersGenerator.Users.Last().Id;

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addRemovePostLikeDTO = new AddRemovePostLikeDTO()
            {
                PostId = postId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/post-likes/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addRemovePostLikeDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task AddPostLikeAsyncTestReturnsOK()
        {
            // Arrange
            var postId = _fakePostsGenerator.Posts.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addRemovePostLikeDTO = new AddRemovePostLikeDTO()
            {
                PostId = postId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/post-likes/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addRemovePostLikeDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var postLikeJson = await response.Content.ReadAsStringAsync();
                var postLike = JsonSerializer.Deserialize<GetPostLikeDTO>(postLikeJson, jsonSerializerOptions)!;
                postLike.UserId.Should().Be(addRemovePostLikeDTO.UserId);
                postLike.PostId.Should().Be(addRemovePostLikeDTO.PostId);
            }
        }
    }
}
