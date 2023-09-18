using AutoMapper;
using ChatService.Application.DTOs.DialogDTOs;
using ChatService.Domain.Entities;

namespace ChatService.Application.AutoMapperProfiles
{
    public class DialogProfile : Profile
    {
        public DialogProfile()
        {
            CreateMap<Dialog, GetDialogDTO>();
        }
    }
}
