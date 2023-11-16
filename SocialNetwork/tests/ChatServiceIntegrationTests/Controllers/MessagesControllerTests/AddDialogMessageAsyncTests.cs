using FluentAssertions;
using MongoDB.Driver;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using ChatService.Application.DTOs.MessageDTOs;

namespace ChatServiceIntegrationTests.Controllers.MessagesControllerTests
{
    public class AddDialogMessageAsyncTests : ControllerTests
    {
        [Fact]
        public async Task AddDialogMessageAsyncTestReturnsForbidden()
        {
            // Arrange
            var senderId = _fakeUsersGenerator.Users.First().Id;
            var receiverId = _fakeUsersGenerator.Users[1].Id;
            var authenticatedUserId = _fakeUsersGenerator.Users.Last().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, authenticatedUserId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addDialogMessageDTO = new AddDialogMessageDTO
            {
                Text = "text",
                SenderId = senderId,
                ReceiverId = receiverId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/dialogs/messages");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addDialogMessageDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AddDialogMessageAsyncTestReturnsNotFound()
        {
            // Arrange
            var senderId = _fakeUsersGenerator.Users.First().Id;
            var receiverId = Guid.NewGuid();
            var authenticatedUserId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, authenticatedUserId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addDialogMessageDTO = new AddDialogMessageDTO
            {
                Text = "text",
                SenderId = senderId,
                ReceiverId = receiverId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/dialogs/messages");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addDialogMessageDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AddDialogMessageAsyncTestReturnsNoContent()
        {
            // Arrange
            var senderId = _fakeUsersGenerator.Users.First().Id;
            var receiverId = _fakeUsersGenerator.Users[1].Id;
            var authenticatedUserId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, authenticatedUserId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var addDialogMessageDTO = new AddDialogMessageDTO
            {
                Text = "text",
                SenderId = senderId,
                ReceiverId = receiverId
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/dialogs/messages");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addDialogMessageDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
