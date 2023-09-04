using AutoMapper;
using IdentityService.BLL.DTOs.RoleDTOs;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.PL.AutoMapperProfiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<IdentityRole, GetRoleDTO>();
            CreateMap<AddRoleDTO, IdentityRole>();
            CreateMap<UpdateRoleDTO, IdentityRole>();
        }
    }
}
