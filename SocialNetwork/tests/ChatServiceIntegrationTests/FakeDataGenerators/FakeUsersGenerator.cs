using Bogus;
using ChatService.Domain.Entities;

namespace ChatServiceIntegrationTests.FakeDataGenerators
{
    public class FakeUsersGenerator
    {
        public readonly List<User> Users = new();

        private readonly int _amountOfUsers = 10;

        public void InitializeData()
        {
            var userGenerator = GetUserGenerator();
            var fakeUsers = userGenerator.Generate(_amountOfUsers);
            Users.AddRange(fakeUsers);
        }

        private Faker<User> GetUserGenerator()
        {
            return new Faker<User>()
                .RuleFor(user => user.Id, _ => Guid.NewGuid())
                .RuleFor(user => user.FirstName, faker => faker.Name.FirstName())
                .RuleFor(user => user.LastName, faker => faker.Name.LastName())
                .RuleFor(user => user.Image, faker => faker.Internet.Avatar())
                .RuleFor(user => user.BirthDate, faker => faker.Date.PastDateOnly());
        }
    }
}
