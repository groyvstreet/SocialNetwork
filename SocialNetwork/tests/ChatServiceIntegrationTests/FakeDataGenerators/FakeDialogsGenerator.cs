using Bogus;
using ChatService.Domain.Entities;

namespace ChatServiceIntegrationTests.FakeDataGenerators
{
    public class FakeDialogsGenerator
    {
        public readonly List<Dialog> Dialogs = new();

        private readonly int _amountOfDialogs = 1;

        public void InitializeData(List<User> users, List<Message> messages)
        {
            var dialogGenerator = GetDialogGenerator(users, messages);
            var fakeDialogs = dialogGenerator.Generate(_amountOfDialogs);
            Dialogs.AddRange(fakeDialogs);
        }

        private Faker<Dialog> GetDialogGenerator(List<User> users, List<Message> messages)
        {
            return new Faker<Dialog>()
                .RuleFor(dialog => dialog.Id, _ => Guid.NewGuid())
                .RuleFor(dialog => dialog.Users, _ => users)
                .RuleFor(dialog => dialog.Messages, _ => messages);
        }
    }
}
