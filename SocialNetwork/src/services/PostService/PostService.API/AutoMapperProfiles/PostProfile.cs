using AutoMapper;
using PostService.Application.DTOs.PostDTOs;
using PostService.Domain.Entities;

namespace PostService.API.AutoMapperProfiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Post, GetPostDTO>();
            CreateMap<AddPostDTO, Post>();
        }
    }
}
