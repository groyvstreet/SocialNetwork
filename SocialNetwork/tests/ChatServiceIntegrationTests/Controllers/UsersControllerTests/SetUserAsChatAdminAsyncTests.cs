using ChatService.Application.DTOs.ChatDTOs;
using FluentAssertions;
using MongoDB.Driver;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace ChatServiceIntegrationTests.Controllers.UsersControllerTests
{
    public class SetUserAsChatAdminAsyncTests : ControllerTests
    {
        [Fact]
        public async Task SetUserAsChatAdminAsyncTestReturnsForbidden()
        {
            // Arrange
            var chatId = _fakeChatsGenerator.Chats.First().Id;
            var userId = _fakeUsersGenerator.Users[1].Id;
            var authenticatedUserId = _fakeUsersGenerator.Users.Last().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, authenticatedUserId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var setUserAsChatAdminDTO = new SetUserAsChatAdminDTO
            {
                ChatId = chatId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/chats/admins/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(setUserAsChatAdminDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task SetUserAsChatAdminAsyncTestReturnsNoContent()
        {
            // Arrange
            var chatId = _fakeChatsGenerator.Chats.First().Id;
            var userId = _fakeUsersGenerator.Users[1].Id;
            var authenticatedUserId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, authenticatedUserId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var setUserAsChatAdminDTO = new SetUserAsChatAdminDTO
            {
                ChatId = chatId,
                UserId = userId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/chats/admins/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(setUserAsChatAdminDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
