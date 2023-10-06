using ChatService.Application;
using ChatService.Application.Interfaces.Repositories;
using ChatService.Domain.Entities;
using ChatService.Infrastructure.Interfaces;

namespace ChatService.Infrastructure.Services
{
    public class UserKafkaConsumerHandler : IKafkaConsumerHandler<RequestOperation, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly IDialogRepository _dialogRepository;
        private readonly IChatRepository _chatRepository;

        public UserKafkaConsumerHandler(IUserRepository userRepository,
                                        IDialogRepository dialogRepository,
                                        IChatRepository chatRepository)
        {
            _userRepository = userRepository;
            _dialogRepository = dialogRepository;
            _chatRepository = chatRepository;
        }

        public async Task HandleAsync(RequestOperation operation, User user)
        {
            switch (operation)
            {
                case RequestOperation.Create:
                    await _userRepository.AddAsync(user);
                    break;
                case RequestOperation.Update:
                    var userForUpdate = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == user.Id);

                    if (userForUpdate is null)
                    {
                        await _userRepository.AddAsync(user);
                    }
                    else
                    {
                        await _userRepository.UpdateAsync(user);
                        await _dialogRepository.UpdateUserAsync(user);
                        await _chatRepository.UpdateUserAsync(user);
                    }

                    break;
                case RequestOperation.Remove:
                    var userForRemove = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == user.Id);

                    if (userForRemove is not null)
                    {
                        await _userRepository.RemoveAsync(user);
                        await _dialogRepository.RemoveUserAsync(user);
                        await _chatRepository.RemoveUserAsync(user);
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
