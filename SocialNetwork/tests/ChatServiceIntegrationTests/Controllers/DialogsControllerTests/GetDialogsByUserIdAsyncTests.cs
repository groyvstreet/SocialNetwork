using MongoDB.Driver;
using System.Security.Claims;
using System.Text.Json;
using FluentAssertions;
using System.Net;
using FluentAssertions.Execution;
using ChatService.Application.DTOs.DialogDTOs;

namespace ChatServiceIntegrationTests.Controllers.DialogsControllerTests
{
    public class GetDialogsByUserIdAsyncTests : ControllerTests
    {
        [Fact]
        public async Task GetDialogsByUserIdAsyncTestReturnsForbidden()
        {
            // Arrange
            var userId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/dialogs?userId={userId}");
            request.Headers.Add("Authorization", $"Bearer {token}");
            
            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetDialogsByUserIdAsyncTestReturnsOK()
        {
            // Arrange
            var userId = _fakeUsersGenerator.Users.First().Id;
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var token = JwtGenerator.GenerateToken(claims);

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/dialogs?userId={userId}");
            request.Headers.Add("Authorization", $"Bearer {token}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var dialogsJson = await response.Content.ReadAsStringAsync();
                var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dialogs = JsonSerializer.Deserialize<List<GetDialogDTO>>(dialogsJson, jsonSerializerOptions)!;
                dialogs.Should().Contain(dialog => dialog.Users.Any(user => user.Id == userId));
            }
        }
    }
}
