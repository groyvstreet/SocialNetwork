using AutoMapper;
using PostService.Application.DTOs.CommentLikeDTOs;
using PostService.Domain.Entities;

namespace PostService.API.AutoMapperProfiles
{
    public class CommentLikeProfile : Profile
    {
        public CommentLikeProfile()
        {
            CreateMap<CommentLike, GetCommentLikeDTO>();
            CreateMap<AddCommentLikeDTO, CommentLike>();
        }
    }
}
