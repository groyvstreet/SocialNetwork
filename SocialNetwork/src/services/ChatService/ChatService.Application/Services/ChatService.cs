using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;

namespace ChatService.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task RemoveEmptyChatsAsync()
        {
            await _chatRepository.RemoveRangeAsync(c => !c.Users.Any());
        }
    }
}
