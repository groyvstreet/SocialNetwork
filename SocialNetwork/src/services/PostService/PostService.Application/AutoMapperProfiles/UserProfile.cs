using AutoMapper;
using PostService.Application.DTOs.UserDTOs;
using PostService.Domain.Entities;

namespace PostService.Application.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, GetUserDTO>();
        }
    }
}
