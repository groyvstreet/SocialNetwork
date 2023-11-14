using ChatService.Application.DTOs.ChatDTOs;
using FluentAssertions;
using MongoDB.Driver;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace ChatServiceIntegrationTests.Controllers.ChatsControllerTests
{
    public class UpdateChatAsyncTests : ControllerTests
    {
        [Fact]
        public async Task UpdateChatAsyncTestReturnsNotFound()
        {
            // Arrange
            var chatId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);
            var updateChatDTO = new UpdateChatDTO
            {
                Id = chatId,
                Name = "name",
                Image = "image"
            };

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/chats/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updateChatDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateChatAsyncTestReturnsForbidden()
        {
            // Arrange
            var chatId = _fakeChatsGenerator.Chats.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);
            var updateChatDTO = new UpdateChatDTO
            {
                Id = chatId,
                Name = "name",
                Image = "image"
            };

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/chats/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updateChatDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateChatAsyncTestReturnsNoContent()
        {
            // Arrange
            var chatId = _fakeChatsGenerator.Chats.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var updateChatDTO = new UpdateChatDTO
            {
                Id = chatId,
                Name = "name",
                Image = "image"
            };

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/chats/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updateChatDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
