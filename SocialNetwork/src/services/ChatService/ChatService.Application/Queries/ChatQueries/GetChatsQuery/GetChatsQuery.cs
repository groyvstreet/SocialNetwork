using ChatService.Application.DTOs.ChatDTOs;
using MediatR;

namespace ChatService.Application.Queries.ChatQueries.GetChatsQuery
{
    public class GetChatsQuery : IRequest<List<GetChatDTO>>
    {
        public Guid UserId { get; set; }
    }
}
