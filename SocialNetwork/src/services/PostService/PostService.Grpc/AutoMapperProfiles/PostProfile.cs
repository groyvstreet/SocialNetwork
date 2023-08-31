using AutoMapper;
using PostService.Application.DTOs.PostDTOs;

namespace PostService.Grpc.AutoMapperProfiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Domain.Entities.Post, Post>();
            CreateMap<Post, AddPostDTO>();
            CreateMap<AddPostDTO, Domain.Entities.Post>();
            CreateMap<Domain.Entities.Post, GetPostDTO>();
            CreateMap<GetPostDTO, Post>();
        }
    }
}
