using ChatService.Application.DTOs.ChatDTOs;
using FluentAssertions;
using MongoDB.Driver;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace ChatServiceIntegrationTests.Controllers.UsersControllerTests
{
    public class AddUserToChatAsyncTests : ControllerTests
    {
        [Fact]
        public async Task AddUserToChatAsyncTestReturnsForbidden()
        {
            // Arrange
            var chatId = _fakeChatsGenerator.Chats.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;
            var invitedUserId = _fakeUsersGenerator.Users.Last().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addUserToChatDTO = new AddUserToChatDTO
            {
                ChatId = chatId,
                UserId = userId,
                InvitedUserId = invitedUserId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/chats/users/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addUserToChatDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AddUserToChatAsyncTestReturnsForbidden2()
        {
            // Arrange
            var chatId = _fakeChatsGenerator.Chats.First().Id;
            var userId = _fakeUsersGenerator.Users[4].Id;
            var invitedUserId = _fakeUsersGenerator.Users.Last().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addUserToChatDTO = new AddUserToChatDTO
            {
                ChatId = chatId,
                UserId = userId,
                InvitedUserId = invitedUserId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/chats/users/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addUserToChatDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            
            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AddUserToChatAsyncTestReturnsNotFound()
        {
            // Arrange
            var chatId = _fakeChatsGenerator.Chats.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;
            var invitedUserId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addUserToChatDTO = new AddUserToChatDTO
            {
                ChatId = chatId,
                UserId = userId,
                InvitedUserId = invitedUserId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/chats/users/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addUserToChatDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddUserToChatAsyncTestReturnsNoContent()
        {
            // Arrange
            var chatId = _fakeChatsGenerator.Chats.First().Id;
            var userId = _fakeUsersGenerator.Users.First().Id;
            var invitedUserId = _fakeUsersGenerator.Users.Last().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addUserToChatDTO = new AddUserToChatDTO
            {
                ChatId = chatId,
                UserId = userId,
                InvitedUserId = invitedUserId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/chats/users/");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addUserToChatDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
