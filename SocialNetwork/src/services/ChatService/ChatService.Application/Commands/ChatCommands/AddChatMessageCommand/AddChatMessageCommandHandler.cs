using ChatService.Application.DTOs.MessageDTOs;
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
        private readonly IBackgroundJobService _backgroundJobService;

        public AddChatMessageCommandHandler(IChatRepository chatRepository,
                                            IUserRepository userRepository,
                                            IChatNotificationService chatNotificationService,
                                            IBackgroundJobService backgroundJobService)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _chatNotificationService = chatNotificationService;
            _backgroundJobService = backgroundJobService;
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

            var user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == DTO.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {DTO.UserId}");
            }

            if (!chat.Users.Any(u => u.Id == DTO.UserId))
            {
                throw new ForbiddenException($"no such user with id = {DTO.UserId} in chat with id = {DTO.ChatId}");
            }

            if (request.DateTime is null)
            {
                await AddChatMessageAsync(request.DTO);
            }
            else
            {
                _backgroundJobService.AddSchedule(() => AddChatMessageAsync(request.DTO), request.DateTime.Value - DateTimeOffset.Now);
            }

            return new Unit();
        }

        public async Task AddChatMessageAsync(AddChatMessageDTO DTO)
        {
            var chat = await _chatRepository.GetFirstOrDefaultByAsync(c => c.Id == DTO.ChatId);

            if (chat is null)
            {
                return;
            }

            var user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == DTO.UserId);

            if (user is null)
            {
                return;
            }

            if (!chat.Users.Any(u => u.Id == DTO.UserId))
            {
                return;
            }

            var message = new Message
            {
                DateTime = DateTimeOffset.Now,
                Text = DTO.Text,
                User = user
            };
            await _chatRepository.AddChatMessageAsync(DTO.ChatId, message);

            await _chatNotificationService.SendMessageAsync(chat, message);
        }
    }
}
