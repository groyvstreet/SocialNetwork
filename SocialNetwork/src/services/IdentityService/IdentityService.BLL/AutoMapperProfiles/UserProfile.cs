using AutoMapper;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.DAL.Entities;

namespace IdentityService.BLL.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<DateTime, DateOnly>().ConstructUsing(dt => DateOnly.FromDateTime(dt));
            CreateMap<DateOnly, DateTime>().ConstructUsing(d => d.ToDateTime(TimeOnly.MinValue));
            CreateMap<User, GetUserDTO>();
            CreateMap<AddUserDTO, User>();
            CreateMap<UpdateUserDTO, User>();
        }
    }
}
