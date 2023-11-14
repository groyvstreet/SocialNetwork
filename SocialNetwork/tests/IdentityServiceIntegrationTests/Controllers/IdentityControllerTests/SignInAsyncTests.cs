using FluentAssertions;
using System.Net;

namespace IdentityServiceIntegrationTests.Controllers.IdentityControllerTests
{
    public class SignInAsyncTests : ControllerTests
    {
        [Fact]
        public async Task SignInAsyncTestReturnsOK()
        {
            // Arrange
            var user = _fakeUsersGenerator.Users.First();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/signin?email={user.Email}&password=string");
            
            // Act
            var response = await _httpClient.SendAsync(request);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task SignInAsyncTestReturnsUnauthorized()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/signin?email=string&password=string");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
