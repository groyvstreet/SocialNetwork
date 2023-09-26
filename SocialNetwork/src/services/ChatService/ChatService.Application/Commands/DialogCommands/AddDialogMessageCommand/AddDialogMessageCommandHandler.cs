﻿using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Application.Interfaces.Services;
using ChatService.Domain.Entities;
using MediatR;

namespace ChatService.Application.Commands.DialogCommands.AddDialogMessageCommand
{
    public class AddDialogMessageCommandHandler : IRequestHandler<AddDialogMessageCommand>
    {
        private readonly IDialogRepository _dialogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDialogNotificationService _dialogNotificationService;

        public AddDialogMessageCommandHandler(IDialogRepository dialogRepository,
                                              IUserRepository userRepository,
                                              IDialogNotificationService dialogNotificationService)
        {
            _dialogRepository = dialogRepository;
            _userRepository = userRepository;
            _dialogNotificationService = dialogNotificationService;
        }

        public async Task<Unit> Handle(AddDialogMessageCommand request, CancellationToken cancellationToken)
        {
            var DTO = request.DTO;

            if (DTO.SenderId != request.AuthenticatedUserId)
            {
                throw new ForbiddenException("forbidden");
            }

            var sender = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == DTO.SenderId);

            if (sender is null)
            {
                throw new NotFoundException($"no such user with id = {DTO.SenderId}");
            }

            var receiver = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == DTO.ReceiverId);

            if (receiver is null)
            {
                throw new NotFoundException($"no such user with id = {DTO.ReceiverId}");
            }

            var dialog = await _dialogRepository.GetFirstOrDefaultByAsync(d =>
                d.Users.Any(u => u.Id == DTO.SenderId) && d.Users.Any(u => u.Id == DTO.ReceiverId));

            if (dialog is null)
            {
                dialog = new Dialog();
                dialog.Users.AddRange(new List<User> { sender, receiver });
                await _dialogRepository.AddAsync(dialog);
            }

            var message = new Message
            {
                DateTime = DateTimeOffset.Now,
                Text = DTO.Text,
                User = sender
            };
            await _dialogRepository.AddDialogMessageAsync(dialog.Id, message);

            await _dialogNotificationService.SendMessageAsync(dialog, message);

            return new Unit();
        }
    }
}
