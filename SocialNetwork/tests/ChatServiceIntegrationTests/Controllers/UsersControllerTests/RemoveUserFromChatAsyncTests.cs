using ChatService.Application.DTOs.ChatDTOs;
using FluentAssertions;
using MongoDB.Driver;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace ChatServiceIntegrationTests.Controllers.UsersControllerTests
{
    public class RemoveUserFromChatAsyncTests : ControllerTests
    {
        [Fact]
        public async Task RemoveUserFromChatAsyncTestReturnsForbidden()
        {
            // Arrange
            var chatId = _fakeChatsGenerator.Chats.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;
            var authenticatedUserId = _fakeUsersGenerator.Users[1].Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, authenticatedUserId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var removeUserFromChatDTO = new RemoveUserFromChatDTO
            {
                ChatId = chatId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/chats/users/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(removeUserFromChatDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task RemoveUserFromChatAsyncTestReturnsNotFound()
        {
            // Arrange
            var chatId = _fakeChatsGenerator.Chats.First().Id;
            var userId = _fakeUsersGenerator.Users.Last().Id;
            var authenticatedUserId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, authenticatedUserId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var removeUserFromChatDTO = new RemoveUserFromChatDTO
            {
                ChatId = chatId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/chats/users/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(removeUserFromChatDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task RemoveUserFromChatAsyncTestReturnsNoContent()
        {
            // Arrange
            var chatId = _fakeChatsGenerator.Chats.First().Id;
            var userId = _fakeUsersGenerator.Users[1].Id;
            var authenticatedUserId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, authenticatedUserId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var removeUserFromChatDTO = new RemoveUserFromChatDTO
            {
                ChatId = chatId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/chats/users/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(removeUserFromChatDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
