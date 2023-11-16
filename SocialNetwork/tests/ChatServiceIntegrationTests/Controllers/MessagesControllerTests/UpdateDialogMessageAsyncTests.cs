using ChatService.Application.DTOs.MessageDTOs;
using FluentAssertions;
using MongoDB.Driver;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace ChatServiceIntegrationTests.Controllers.MessagesControllerTests
{
    public class UpdateDialogMessageAsyncTests : ControllerTests
    {
        [Fact]
        public async Task UpdateDialogMessageAsyncTestReturnsForbidden()
        {
            // Arrange
            var dialogId = _fakeDialogsGenerator.Dialogs.First().Id;
            var messageId = _fakeMessagesGenerator.Messages.First().Id;
            var authenticatedUserId = _fakeUsersGenerator.Users.Last().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, authenticatedUserId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var updateDialogMessageDTO = new UpdateDialogMessageDTO
            {
                DialogId = dialogId,
                MessageId = messageId,
                Text = "text"
            };

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/dialogs/messages");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updateDialogMessageDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateDialogMessageAsyncTestReturnsDialogNotFound()
        {
            // Arrange
            var dialogId = Guid.NewGuid();
            var messageId = _fakeMessagesGenerator.Messages.First().Id;
            var authenticatedUserId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, authenticatedUserId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var updateDialogMessageDTO = new UpdateDialogMessageDTO
            {
                DialogId = dialogId,
                MessageId = messageId,
                Text = "text"
            };

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/dialogs/messages");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updateDialogMessageDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateDialogMessageAsyncTestReturnsMessageNotFound()
        {
            // Arrange
            var dialogId = _fakeDialogsGenerator.Dialogs.First().Id;
            var messageId = Guid.NewGuid();
            var authenticatedUserId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, authenticatedUserId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var updateDialogMessageDTO = new UpdateDialogMessageDTO
            {
                DialogId = dialogId,
                MessageId = messageId,
                Text = "text"
            };

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/dialogs/messages");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updateDialogMessageDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateDialogMessageAsyncTestReturnsNoContent()
        {
            // Arrange
            var dialogId = _fakeDialogsGenerator.Dialogs.First().Id;
            var messageId = _fakeMessagesGenerator.Messages.First().Id;
            var authenticatedUserId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, authenticatedUserId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var updateDialogMessageDTO = new UpdateDialogMessageDTO
            {
                DialogId = dialogId,
                MessageId = messageId,
                Text = "text"
            };

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/dialogs/messages");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updateDialogMessageDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
