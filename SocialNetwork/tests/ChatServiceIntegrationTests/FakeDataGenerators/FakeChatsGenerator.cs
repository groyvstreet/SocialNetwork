using Bogus;
using ChatService.Domain.Entities;

namespace ChatServiceIntegrationTests.FakeDataGenerators
{
    public class FakeChatsGenerator
    {
        public readonly List<Chat> Chats = new();

        private readonly int _amountOfChats = 1;

        public void InitializeData(List<User> users, List<Message> messages)
        {
            var chatGenerator = GetChatGenerator(users, messages);
            var fakeChats = chatGenerator.Generate(_amountOfChats);
            Chats.AddRange(fakeChats);
        }

        private Faker<Chat> GetChatGenerator(List<User> users, List<Message> messages)
        {
            var chatUsers = users.Select(user => new ChatUser
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Image = user.Image,
                BirthDate = user.BirthDate,
                IsAdmin = false
            }).ToList();

            chatUsers[0].IsAdmin = true;
            chatUsers[0].InvitedUsers = users.Skip(1).Take(users.Count - 1).Select(user => user.Id.ToString()).ToList();

            return new Faker<Chat>()
                .RuleFor(chat => chat.Id, _ => Guid.NewGuid())
                .RuleFor(chat => chat.Name, fake => fake.Lorem.Word())
                .RuleFor(chat => chat.Image, fake => fake.Internet.Avatar())
                .RuleFor(chat => chat.Users, _ => chatUsers)
                .RuleFor(chat => chat.Messages, _ => messages);
        }
    }
}
