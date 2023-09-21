﻿using MediatR;

namespace ChatService.Application.Commands.ChatCommands.UpdateChatMessageCommand
{
    public class UpdateChatMessageCommand : IRequest
    {
        public Guid ChatId { get; set; }

        public string Text { get; set; } = string.Empty;

        public Guid MessageId { get; set; }
    }
}