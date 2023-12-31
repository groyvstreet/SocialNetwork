﻿using AutoMapper;
using PostService.Application.DTOs.CommentDTOs;
using PostService.Domain.Entities;

namespace PostService.Application.AutoMapperProfiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, GetCommentDTO>();
            CreateMap<AddCommentDTO, Comment>();
            CreateMap<UpdateCommentDTO, Comment>();
        }
    }
}
