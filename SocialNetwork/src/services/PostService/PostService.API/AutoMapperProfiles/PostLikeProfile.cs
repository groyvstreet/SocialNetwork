using AutoMapper;
using PostService.Application.DTOs.PostLikeDTOs;
using PostService.Domain.Entities;

namespace PostService.API.AutoMapperProfiles
{
    public class PostLikeProfile : Profile
    {
        public PostLikeProfile()
        {
            CreateMap<PostLike, GetPostLikeDTO>();
            CreateMap<AddRemovePostLikeDTO, PostLike>();
        }
    }
}
