using ChatService.Domain.Entities;
using MediatR;

namespace ChatService.Application.Queries.ChatQueries.GetChatsQuery
{
    public class GetChatsQuery : IRequest<List<Chat>>
    {
        public Guid UserId { get; set; }
    }
}
