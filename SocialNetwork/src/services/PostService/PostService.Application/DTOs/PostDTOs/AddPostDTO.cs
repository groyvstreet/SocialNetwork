﻿namespace PostService.Application.DTOs.PostDTOs
{
    public class AddPostDTO
    {
        public string Text { get; set; } = string.Empty;

        public Guid UserId { get; set; }
    }
}
