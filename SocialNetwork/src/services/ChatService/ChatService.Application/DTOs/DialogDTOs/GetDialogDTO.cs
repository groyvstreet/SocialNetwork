using ChatService.Application.DTOs.MessageDTOs;
using ChatService.Domain.Entities;

namespace ChatService.Application.DTOs.DialogDTOs
{
    public class GetDialogDTO
    {
        public Guid Id { get; set; }

        public ulong MessageCount { get; set; }

        public List<GetMessageDTO> Messages { get; set; }

        public List<User> Users { get; set; }
    }
}
