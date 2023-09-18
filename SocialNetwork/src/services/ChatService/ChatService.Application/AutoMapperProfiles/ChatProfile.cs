using AutoMapper;
using ChatService.Application.DTOs.ChatDTOs;
using ChatService.Domain.Entities;

namespace ChatService.Application.AutoMapperProfiles
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<Chat, GetChatDTO>();
        }
    }
}
