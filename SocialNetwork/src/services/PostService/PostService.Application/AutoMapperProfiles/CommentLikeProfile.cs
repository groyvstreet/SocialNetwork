using AutoMapper;
using PostService.Application.DTOs.CommentLikeDTOs;
using PostService.Domain.Entities;

namespace PostService.Application.AutoMapperProfiles
{
    public class CommentLikeProfile : Profile
    {
        public CommentLikeProfile()
        {
            CreateMap<CommentLike, GetCommentLikeDTO>();
            CreateMap<AddRemoveCommentLikeDTO, CommentLike>();
        }
    }
}
