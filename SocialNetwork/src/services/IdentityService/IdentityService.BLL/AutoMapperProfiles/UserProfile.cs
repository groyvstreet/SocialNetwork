﻿using AutoMapper;
using IdentityService.BLL.DTOs.UserDTOs;
using IdentityService.DAL.Entities;

namespace IdentityService.BLL.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, GetUserDTO>();
            CreateMap<AddUserDTO, User>();
            CreateMap<UpdateUserDTO, User>();
        }
    }
}
