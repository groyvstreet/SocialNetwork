using Bogus;
using ChatService.Domain.Entities;

namespace ChatServiceIntegrationTests.FakeDataGenerators
{
    public class FakeMessagesGenerator
    {
        public readonly List<Message> Messages = new();

        private readonly int _amountOfMessages = 1;

        public void InitializeData(User user)
        {
            var messageGenerator = GetMessageGenerator(user);
            var fakeMessages = messageGenerator.Generate(_amountOfMessages);
            Messages.AddRange(fakeMessages);
        }

        private Faker<Message> GetMessageGenerator(User user)
        {
            return new Faker<Message>()
                .RuleFor(message => message.Id, _ => Guid.NewGuid())
                .RuleFor(message => message.DateTime, faker => faker.Date.PastOffset())
                .RuleFor(message => message.Text, faker => faker.Lorem.Text())
                .RuleFor(message => message.User, _ => user);
        }
    }
}
