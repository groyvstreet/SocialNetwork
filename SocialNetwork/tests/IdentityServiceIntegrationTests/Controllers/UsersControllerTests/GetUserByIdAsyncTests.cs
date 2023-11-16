using FluentAssertions;
using FluentAssertions.Execution;
using IdentityService.BLL.DTOs.UserDTOs;
using System.Net;
using System.Text.Json;

namespace IdentityServiceIntegrationTests.Controllers.UsersControllerTests
{
    public class GetUserByIdAsyncTests : ControllerTests
    {
        [Fact]
        public async Task GetUserByIdAsyncTestReturnsHttpOK()
        {
            // Arrange
            var userId = "5";

            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/users/{userId}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var userJson = await response.Content.ReadAsStringAsync();
                var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var user = JsonSerializer.Deserialize<GetUserDTO>(userJson, jsonSerializerOptions)!;
                user.Id.Should().Be(userId);
            }
        }

        [Fact]
        public async Task GetUserByIdAsyncTestReturnsHttpNotFound()
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod("GET"), $"/api/users/{Guid.NewGuid()}");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
