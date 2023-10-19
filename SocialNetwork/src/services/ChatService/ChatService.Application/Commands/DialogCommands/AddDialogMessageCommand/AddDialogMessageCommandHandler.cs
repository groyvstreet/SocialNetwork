using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using ChatService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ChatService.Application.Commands.DialogCommands.AddDialogMessageCommand
{
    public class AddDialogMessageCommandHandler : IRequestHandler<AddDialogMessageCommand>
    {
        private readonly IDialogRepository _dialogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDialogNotificationService _dialogNotificationService;
        private readonly IPostService _postService;
        private readonly ILogger<AddDialogMessageCommandHandler> _logger;

        public AddDialogMessageCommandHandler(IDialogRepository dialogRepository,
                                              IUserRepository userRepository,
                                              IDialogNotificationService dialogNotificationService,
                                              IPostService postService,
                                              ILogger<AddDialogMessageCommandHandler> logger)
        {
            _dialogRepository = dialogRepository;
            _userRepository = userRepository;
            _dialogNotificationService = dialogNotificationService;
            _postService = postService;
            _logger = logger;
        }

        public async Task<Unit> Handle(AddDialogMessageCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;

            if (DTO.SenderId != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
            }

            var sender = await _userRepository.GetFirstOrDefaultByAsync(user => user.Id == DTO.SenderId);

            if (sender is null)
            {
                throw new NotFoundException($"no such user with id = {DTO.SenderId}");
            }

            var receiver = await _userRepository.GetFirstOrDefaultByAsync(user => user.Id == DTO.ReceiverId);

            if (receiver is null)
            {
                throw new NotFoundException($"no such user with id = {DTO.ReceiverId}");
            }

            var dialog = await _dialogRepository.GetFirstOrDefaultByAsync(dialog =>
                dialog.Users.Any(user => user.Id == DTO.SenderId) && dialog.Users.Any(user => user.Id == DTO.ReceiverId));

            if (dialog is null)
            {
                dialog = new Dialog();
                dialog.Users.AddRange(new List<User> { sender, receiver });
                await _dialogRepository.AddAsync(dialog);
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
                User = sender
            };
            await _dialogRepository.AddDialogMessageAsync(dialog.Id, message);

            await _dialogNotificationService.SendMessageAsync(dialog, message);

            _logger.LogInformation("message - {message} added to dialog with id {id}", JsonSerializer.Serialize(message), dialog.Id);

            return new Unit();
        }
    }
}
