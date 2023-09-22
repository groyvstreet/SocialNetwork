using ChatService.Application.DTOs.DialogDTOs;
using MediatR;

namespace ChatService.Application.Queries.DialogQueries.GetDialogsQuery
{
    public class GetDialogsQuery : IRequest<List<GetDialogDTO>>
    {
        public Guid UserId { get; set; }

        public Guid AuthenticatedUserId { get; set; }

        public GetDialogsQuery(Guid userId, Guid authenticatedUserId)
        {
            UserId = userId;
            AuthenticatedUserId = authenticatedUserId;
        }
    }
}
