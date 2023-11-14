using FluentAssertions.Execution;
using FluentAssertions;
using MongoDB.Driver;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using ChatService.Application.DTOs.ChatDTOs;

namespace ChatServiceIntegrationTests.Controllers.ChatsControllerTests
{
    public class GetChatsByUserIdAsyncTests : ControllerTests
    {
        [Fact]
        public async Task GetChatsByUserIdAsyncTestReturnsForbidden()
        {
            // Arrange
            var userId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/chats?userId={userId}");
            request.Headers.Add("Authorization", $"Bearer {token}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetChatsByUserIdAsyncTestReturnsOK()
        {
            // Arrange
            var userId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/chats?userId={userId}");
            request.Headers.Add("Authorization", $"Bearer {token}");
            
            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var chatsJson = await response.Content.ReadAsStringAsync();
                var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var chats = JsonSerializer.Deserialize<List<GetChatDTO>>(chatsJson, jsonSerializerOptions)!;
                chats.Should().Contain(chat => chat.Users.Any(user => user.Id == userId));
            }
        }
    }
}
