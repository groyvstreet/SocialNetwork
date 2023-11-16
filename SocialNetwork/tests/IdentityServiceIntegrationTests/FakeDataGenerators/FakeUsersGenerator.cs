using Bogus;
using IdentityService.DAL.Entities;

namespace IdentityServiceIntegrationTests.FakeDataGenerators
{
    public class FakeUsersGenerator
    {
        public readonly List<User> Users = new();

        private readonly int _amountOfUsers = 10;
        private int _counter = 0;

        public void InitializeData()
        {
            var userGenerator = GetUserGenerator();
            var fakeUsers = userGenerator.Generate(_amountOfUsers);
            Users.AddRange(fakeUsers);
        }

        private Faker<User> GetUserGenerator()
        {
            return new Faker<User>()
                .RuleFor(user => user.Id, _ => (++_counter).ToString())
                .RuleFor(user => user.Email, faker => faker.Internet.Email())
                .RuleFor(user => user.UserName, faker => faker.Internet.UserName())
                .RuleFor(user => user.FirstName, faker => faker.Name.FirstName())
                .RuleFor(user => user.LastName, faker => faker.Name.LastName())
                .RuleFor(user => user.Image, faker => faker.Internet.Avatar())
                .RuleFor(user => user.BirthDate, faker => faker.Date.Past());
        }
    }
}
