﻿namespace ChatService.Application.DTOs.ChatDTOs
{
    public class UpdateChatDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;
    }
}
