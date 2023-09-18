using AutoMapper;
using ChatService.Application.DTOs.DialogDTOs;
using ChatService.Application.Exceptions;
using ChatService.Application.Interfaces;
using MediatR;

namespace ChatService.Application.Queries.DialogQueries.GetDialogsQuery
{
    public class GetDialogsQueryHandler : IRequestHandler<GetDialogsQuery, List<GetDialogDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IDialogRepository _dialogRepository;
        private readonly IUserRepository _userRepository;

        public GetDialogsQueryHandler(IMapper mapper,
                                      IDialogRepository dialogRepository,
                                      IUserRepository userRepository)
        {
            _mapper = mapper;
            _dialogRepository = dialogRepository;
            _userRepository = userRepository;
        }

        public async Task<List<GetDialogDTO>> Handle(GetDialogsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == request.UserId);

            if (user is null)
            {
                throw new NotFoundException($"no such user with id = {request.UserId}");
            }

            var dialogs = await _dialogRepository.GetDialogsByUserIdAsync(request.UserId);
            var dialogDTOs = dialogs.Select(_mapper.Map<GetDialogDTO>).ToList();

            return dialogDTOs;
        }
    }
}
