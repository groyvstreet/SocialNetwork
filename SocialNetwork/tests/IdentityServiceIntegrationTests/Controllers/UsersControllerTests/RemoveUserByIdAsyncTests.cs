using FluentAssertions.Execution;
using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace IdentityServiceIntegrationTests.Controllers.UsersControllerTests
{
    public class RemoveUserByIdAsyncTests : ControllerTests
    {
        [Fact]
        public async Task RemoveUserByIdAsyncTestReturnsUnauthorized()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/users/{Guid.NewGuid()}");
            
            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            }
        }

        [Fact]
        public async Task RemoveUserByIdAsyncTestReturnsForbidden()
        {
            // Arrange
            var user = _fakeUsersGenerator.Users.First();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/signin?email={user.Email}&password=string");
            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;

            request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/users/{Guid.NewGuid()}");
            request.Headers.Add("Authorization", $"Bearer {token["accessToken"]}");

            // Act
            response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task RemoveUserByIdAsyncTestReturnsNoContent()
        {
            // Arrange
            var user = _fakeUsersGenerator.Users.First();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/signin?email={user.Email}&password=string");
            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;

            request = new HttpRequestMessage(new HttpMethod("DELETE"), $"/api/users/{user.Id}");
            request.Headers.Add("Authorization", $"Bearer {token["accessToken"]}");

            // Act
            response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
