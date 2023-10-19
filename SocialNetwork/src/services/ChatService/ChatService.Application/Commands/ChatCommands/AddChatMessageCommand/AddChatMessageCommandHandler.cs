using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ChatService.Application.Commands.ChatCommands.AddChatMessageCommand
{
    public class AddChatMessageCommandHandler : IRequestHandler<AddChatMessageCommand>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatNotificationService _chatNotificationService;
        private readonly IPostService _postService;
        private readonly ILogger<AddChatMessageCommandHandler> _logger;

        public AddChatMessageCommandHandler(IChatRepository chatRepository,
                                            IUserRepository userRepository,
                                            IChatNotificationService chatNotificationService,
                                            IPostService postService,
                                            ILogger<AddChatMessageCommandHandler> logger)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _chatNotificationService = chatNotificationService;
            _postService = postService;
            _logger = logger;
        }

        public async Task<Unit> Handle(AddChatMessageCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;

            if (DTO.UserId != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
            }

            var chat = await _chatRepository.GetFirstOrDefaultByAsync(chat => chat.Id == DTO.ChatId);

            if (chat is null)
            {
                throw new NotFoundException($"no such chat with id = {DTO.ChatId}");
            }

            var user = await _userRepository.GetFirstOrDefaultByAsync(user => user.Id == DTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {DTO.UserId}");
            }

            if (!chat.Users.Any(user => user.Id == DTO.UserId))
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

            _logger.LogInformation("message - {message} added to chat with id {id}", JsonSerializer.Serialize(message), chat.Id);

            return new Unit();
        }
    }
}
