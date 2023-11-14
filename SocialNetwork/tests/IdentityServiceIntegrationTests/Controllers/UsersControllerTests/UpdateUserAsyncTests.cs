using FluentAssertions.Execution;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using System.Text;
using IdentityService.BLL.DTOs.UserDTOs;

namespace IdentityServiceIntegrationTests.Controllers.UsersControllerTests
{
    public class UpdateUserAsyncTests : ControllerTests
    {
        [Fact]
        public async Task UpdateUserAsyncTestReturnsUnauthorized()
        {
            // Arrange
            var updateUserDTO = new UpdateUserDTO();

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/users/");
            var body = JsonSerializer.Serialize(updateUserDTO);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateUserAsyncTestReturnsForbidden()
        {
            // Arrange
            var user = _fakeUsersGenerator.Users.First();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/signin?email={user.Email}&password=string");
            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;

            var updateUserDTO = new UpdateUserDTO
            {
                FirstName = "first",
                LastName = "last",
                Image = "image",
                BirthDate = DateOnly.FromDateTime(DateTime.Now)
            };

            request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/users/");
            request.Headers.Add("Authorization", $"Bearer {token["accessToken"]}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updateUserDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateUserAsyncTestReturnsBadRequest()
        {
            // Arrange
            var user = _fakeUsersGenerator.Users.First();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/signin?email={user.Email}&password=string");
            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;

            var updateUserDTO = new UpdateUserDTO();

            request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/users/");
            request.Headers.Add("Authorization", $"Bearer {token["accessToken"]}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updateUserDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            response = await _httpClient.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateUserAsyncTestReturnsOK()
        {
            // Arrange
            var user = _fakeUsersGenerator.Users.First();

            var request = new HttpRequestMessage(new HttpMethod("POST"), $"/api/identity/signin?email={user.Email}&password=string");
            var response = await _httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<Dictionary<string, string>>(json)!;

            var updateUserDTO = new UpdateUserDTO
            {
                Id = user.Id,
                FirstName = "first",
                LastName = "last",
                Image = "image",
                BirthDate = DateOnly.FromDateTime(DateTime.Now)
            };

            request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/users/");
            request.Headers.Add("Authorization", $"Bearer {token["accessToken"]}");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(updateUserDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act
            response = await _httpClient.SendAsync(request);

            // Assert
            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var userJson = await response.Content.ReadAsStringAsync();
                var updatedUser = JsonSerializer.Deserialize<GetUserDTO>(userJson, jsonSerializerOptions)!;
                updatedUser.Id.Should().Be(updateUserDTO.Id);
                updatedUser.FirstName.Should().Be(updateUserDTO.FirstName);
                updatedUser.LastName.Should().Be(updateUserDTO.LastName);
                updatedUser.Image.Should().Be(updateUserDTO.Image);
                updatedUser.BirthDate.Should().Be(updateUserDTO.BirthDate);
            }
        }
    }
}
