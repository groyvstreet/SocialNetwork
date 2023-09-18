using AutoMapper;
using ChatService.Application.DTOs.MessageDTOs;
using ChatService.Domain.Entities;

namespace ChatService.Application.AutoMapperProfiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, GetMessageDTO>();
        }
    }
}
