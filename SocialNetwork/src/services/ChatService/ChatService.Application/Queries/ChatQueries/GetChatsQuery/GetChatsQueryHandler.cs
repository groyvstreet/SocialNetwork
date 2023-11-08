using AutoMapper;
using ChatService.Application.DTOs.ChatDTOs;
using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using MediatR;

namespace ChatService.Application.Queries.ChatQueries.GetChatsQuery
{
    public class GetChatsQueryHandler : IRequestHandler<GetChatsQuery, List<GetChatDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;

        public GetChatsQueryHandler(IMapper mapper,
                                    IChatRepository chatRepository,
                                    IUserRepository userRepository)
        {
            _mapper = mapper;
            _chatRepository = chatRepository;
            _userRepository = userRepository;
        }

        public async Task<List<GetChatDTO>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
        {
            if (request.UserId != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
            }

            var user = await _userRepository.GetFirstOrDefaultByAsync(user => user.Id == request.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {request.UserId}");
            }

            var chats = await _chatRepository.GetChatsByUserIdAsync(request.UserId);
            var chatDTOs = chats.Select(_mapper.Map<GetChatDTO>).ToList();

            return chatDTOs;
        }
    }
}
