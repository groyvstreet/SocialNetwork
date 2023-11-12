using DotNet.Testcontainers.Builders;
using FluentAssertions.Execution;
using FluentAssertions;
using IdentityService.DAL.Entities;
using IdentityServiceIntegrationTests.FakeDataGenerators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;
using Testcontainers.Redis;
using System.Text;
using IdentityService.BLL.DTOs.UserDTOs;
using Testcontainers.Kafka;

namespace IdentityServiceIntegrationTests.Controllers.UsersControllerTests
{
    public class UpdateUserAsyncTests
    {
        private readonly HttpClient _httpClient;
        private readonly FakeUsersGenerator _fakeUsersGenerator;

        public UpdateUserAsyncTests()
        {
            var msSqlServerContainer = new ContainerBuilder()
                .WithName(Guid.NewGuid().ToString("N"))
                .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
                .WithHostname(Guid.NewGuid().ToString("N"))
                .WithExposedPort(1433)
                .WithPortBinding(1433, true)
                .WithEnvironment("SA_PASSWORD", "S3cur3P@ssW0rd!")
                .WithEnvironment("ACCEPT_EULA", "Y")
                .Build();
            var msSqlServerContainerTask = msSqlServerContainer.StartAsync();
            msSqlServerContainerTask.Wait();

            var redisContainer = new RedisBuilder().Build();
            var redisContainerTask = redisContainer.StartAsync();
            redisContainerTask.Wait();

            var kafkaContainer = new KafkaBuilder().Build();
            var kafkaContainerTask = kafkaContainer.StartAsync();
            kafkaContainerTask.Wait();

            var factory = new CustomWebApplicationFactory<Program>(msSqlServerContainer.GetMappedPublicPort(1433),
                redisContainer.GetConnectionString(),
                kafkaContainer.GetBootstrapAddress());

            var scope = factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            _fakeUsersGenerator = new FakeUsersGenerator();
            _fakeUsersGenerator.InitializeData();
            _fakeUsersGenerator.Users.ForEach(user => userManager.CreateAsync(user, "string").Wait());

            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task UpdateUserAsyncTestReturnsUnauthorized()
        {
            var updateUserDTO = new UpdateUserDTO();

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/users/");
            var body = JsonSerializer.Serialize(updateUserDTO);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            }
        }

        [Fact]
        public async Task UpdateUserAsyncTestReturnsForbidden()
        {
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
            response = await _httpClient.SendAsync(request);

            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            }
        }

        [Fact]
        public async Task UpdateUserAsyncTestReturnsBadRequest()
        {
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
            response = await _httpClient.SendAsync(request);

            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }

        [Fact]
        public async Task UpdateUserAsyncTestReturnsOK()
        {
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
            response = await _httpClient.SendAsync(request);

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
