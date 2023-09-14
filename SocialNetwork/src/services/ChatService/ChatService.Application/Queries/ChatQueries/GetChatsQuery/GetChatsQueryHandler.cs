using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using MediatR;

namespace ChatService.Application.Queries.ChatQueries.GetChatsQuery
{
    public class GetChatsQueryHandler : IRequestHandler<GetChatsQuery, List<Chat>>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public GetChatsQueryHandler(IChatRepository chatRepository,
                                    IUserRepository userRepository)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<List<Chat>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == request.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {request.UserId}");
            }

            return await _chatRepository.GetChatsByUserIdAsync(request.UserId);
        }
    }
}
