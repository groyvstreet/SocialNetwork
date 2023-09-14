using ChatService.Domain.Entities;
using MediatR;

namespace ChatService.Application.Queries.DialogQueries.GetDialogsQuery
{
    public class GetDialogsQuery : IRequest<List<Dialog>>
    {
        public Guid UserId { get; set; }
    }
}
