using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using MediatR;

namespace ChatService.Application.Queries.DialogQueries.GetDialogsQuery
{
    public class GetDialogsQueryHandler : IRequestHandler<GetDialogsQuery, List<Dialog>>
    {
        private readonly IDialogRepository _dialogRepository;
        private readonly IUserRepository _userRepository;

        public GetDialogsQueryHandler(IDialogRepository dialogRepository,
                                      IUserRepository userRepository)
        {
            _dialogRepository = dialogRepository;
            _userRepository = userRepository;
        }

        public async Task<List<Dialog>> Handle(GetDialogsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == request.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {request.UserId}");
            }

            var dialogs = await _dialogRepository.GetDialogsByUserIdAsync(request.UserId);

            return dialogs;
        }
    }
}
