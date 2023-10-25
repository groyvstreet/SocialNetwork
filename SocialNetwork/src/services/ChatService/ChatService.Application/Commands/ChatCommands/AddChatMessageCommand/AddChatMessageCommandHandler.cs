using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using ChatService.Domain.Entities;
using MediatR;

namespace ChatService.Application.Commands.ChatCommands.AddChatMessageCommand
{
    public class AddChatMessageCommandHandler : IRequestHandler<AddChatMessageCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatNotificationService _chatNotificationService;
        private readonly ICacheRepository<User> _userCacheRepository;
        private readonly IPostService _postService;

        public AddChatMessageCommandHandler(IChatRepository chatRepository,
                                            IUserRepository userRepository,
                                            IChatNotificationService chatNotificationService,
                                            ICacheRepository<User> userCacheRepository)
                                            IPostService postService)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _chatNotificationService = chatNotificationService;
            _userCacheRepository = userCacheRepository;
            _postService = postService;
        }

        public async Task<Unit> Handle(AddChatMessageCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;

            if (DTO.UserId != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
            }

            var chat = await _chatRepository.GetFirstOrDefaultByAsync(c => c.Id == DTO.ChatId);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {DTO.ChatId}");
            }

            var user = await _userCacheRepository.GetAsync(DTO.UserId.ToString());

            if (user is null)
            {
                user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == DTO.UserId);

                if (user is null)
                {
                    throw new NotFoundException($"no such user with id = {DTO.UserId}");
                }

                await _userCacheRepository.SetAsync(user.Id.ToString(), user);
            }

            if (!chat.Users.Any(u => u.Id == DTO.UserId))
            {
                throw new ForbiddenException($"no such user with id = {DTO.UserId} in chat with id = {DTO.ChatId}");
            }

            if (DTO.PostId is not null)
            {
                var isPostExists = await _postService.IsPostExistsAsync(DTO.PostId.Value);

                if (!isPostExists)
                {
                    throw new NotFoundException($"no such post with id = {DTO.PostId}");
                }

                await _postService.UpdatePostAsync(DTO.PostId.Value);
            }

            var message = new Message
            {
                DateTime = DateTimeOffset.Now,
                Text = DTO.Text,
                PostId = DTO.PostId?.ToString(),
                User = user
            };
            await _chatRepository.AddChatMessageAsync(DTO.ChatId, message);

            await _chatNotificationService.SendMessageAsync(chat, message);

            return new Unit();
        }
    }
}
