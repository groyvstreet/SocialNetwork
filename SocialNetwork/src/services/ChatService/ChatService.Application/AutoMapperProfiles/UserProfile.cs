using AutoMapper;
using ChatService.Domain.Entities;

namespace ChatService.Application.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, ChatUser>();
        }
    }
}
