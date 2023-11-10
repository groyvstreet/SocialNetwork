using Azure;
using DotNet.Testcontainers.Builders;
using FluentAssertions.Execution;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.DAL.Entities;
using IdentityServiceIntegrationTests.FakeDataGenerators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;
using System.Text;
using Testcontainers.Redis;
using FluentAssertions;

namespace IdentityServiceIntegrationTests.Controllers.IdentityControllerTests
{
    public class SignUpAsyncTests
    {
        private readonly HttpClient _httpClient;
        private readonly FakeUsersGenerator _fakeUsersGenerator;

        public SignUpAsyncTests()
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

            var factory = new CustomWebApplicationFactory<Program>(msSqlServerContainer.GetMappedPublicPort(1433),
                redisContainer.GetConnectionString());

            var scope = factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            _fakeUsersGenerator = new FakeUsersGenerator();
            _fakeUsersGenerator.InitializeData();
            _fakeUsersGenerator.Users.ForEach(user => userManager.CreateAsync(user, "string").Wait());

            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task SignUpAsyncTestReturnsOK()
        {
            var addUserDTO = new AddUserDTO
            {
                Email = "email",
                Password = "password",
                FirstName = "first",
                LastName = "last",
                BirthDate = DateOnly.FromDateTime(DateTime.Now)
            };

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/identity/signup");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addUserDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);

                var userJson = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<GetUserDTO>(userJson, jsonSerializerOptions)!;
                user.Id.Should().Be(addUserDTO.Email);
                user.FirstName.Should().Be(addUserDTO.FirstName);
                user.LastName.Should().Be(addUserDTO.LastName);
                user.Image.Should().BeEmpty();
                user.BirthDate.Should().Be(addUserDTO.BirthDate);
            }
        }

        [Fact]
        public async Task SignUpAsyncTestReturnsConflict()
        {
            var user = _fakeUsersGenerator.Users.First();

            var addUserDTO = new AddUserDTO
            {
                Email = user.Email,
                Password = "password",
                FirstName = "first",
                LastName = "last",
                BirthDate = DateOnly.FromDateTime(DateTime.Now)
            };

            var request = new HttpRequestMessage(new HttpMethod("PUT"), $"/api/identity/signup");
            var jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var body = JsonSerializer.Serialize(addUserDTO, jsonSerializerOptions);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);

            using (new AssertionScope())
            {
                response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            }
        }
    }
}
