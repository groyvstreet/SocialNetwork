using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace IdentityServiceIntegrationTests.Controllers.IdentityControllerTests
{
    public class RefreshAsyncTests : ControllerTests
    {
        [Fact]
        public async Task RefreshAsyncTestReturnsOK()
        {
            // Arrange
            var user = _fakeUsersGenerator.Users.First();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/signin?email={user.Email}&password=string");
            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;

            request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/refresh?accessToken={token["accessToken"]}&refreshToken={token["refreshToken"]}");
            
            // Act
            response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task RefreshAsyncTestReturnsInternalServerErrorWithInvalidAccessToken()
        {
            // Arrange
            var user = _fakeUsersGenerator.Users.First();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/signin?email={user.Email}&password=string");
            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;
            token["accessToken"] = "invalid token";

            request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/refresh?accessToken={token["accessToken"]}&refreshToken={token["refreshToken"]}");
            
            // Act
            response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task RefreshAsyncTestReturnsUnauthorizedWithInvalidRefreshToken()
        {
            // Arrange
            var user = _fakeUsersGenerator.Users.First();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/signin?email={user.Email}&password=string");
            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;
            token["refreshToken"] = "invalid token";

            request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/refresh?accessToken={token["accessToken"]}&refreshToken={token["refreshToken"]}");
            
            // Act
            response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
