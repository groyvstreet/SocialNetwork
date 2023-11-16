using FluentAssertions.Execution;
using IdentityService.BLL.DTOs.UserDTOs;
using System.Net;
using System.Text.Json;
using System.Text;
using FluentAssertions;

namespace IdentityServiceIntegrationTests.Controllers.IdentityControllerTests
{
    public class SignUpAsyncTests : ControllerTests
    {
        [Fact]
        public async Task SignUpAsyncTestReturnsOK()
        {
            // Arrange
            var addUserDTO = new AddUserDTO
            {
                Email = "email",
                Password = "password",
                FirstName = "first",
                LastName = "last",
                BirthDate = DateOnly.FromDateTime(DateTime.Now)
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/signup");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addUserDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            
            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var userJson = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<GetUserDTO>(userJson, jsonSerializerOptions)!;
                user.Email.Should().Be(addUserDTO.Email);
                user.FirstName.Should().Be(addUserDTO.FirstName);
                user.LastName.Should().Be(addUserDTO.LastName);
                user.Image.Should().BeEmpty();
                user.BirthDate.Should().Be(addUserDTO.BirthDate);
            }
        }

        [Fact]
        public async Task SignUpAsyncTestReturnsConflict()
        {
            // Arrange
            var user = _fakeUsersGenerator.Users.First();

            var addUserDTO = new AddUserDTO
            {
                Email = user.Email,
                Password = "password",
                FirstName = "first",
                LastName = "last",
                BirthDate = DateOnly.FromDateTime(DateTime.Now)
            };

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/signup");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addUserDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            }
        }
    }
}
