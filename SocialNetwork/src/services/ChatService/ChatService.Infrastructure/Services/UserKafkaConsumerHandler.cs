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
        private readonly ICacheRepository<User> _userCacheRepository;

        public UserKafkaConsumerHandler(IUserRepository userRepository,
                                        IDialogRepository dialogRepository,
                                        IChatRepository chatRepository,
                                        ICacheRepository<User> userCacheRepository)
        {
            _userRepository = userRepository;
            _dialogRepository = dialogRepository;
            _chatRepository = chatRepository;
            _userCacheRepository = userCacheRepository;
        }

        public async Task HandleAsync(RequestOperation operation, User user)
        {
            switch (operation)
            {
                case RequestOperation.Create:
                    await _userRepository.AddAsync(user);

                    await _userCacheRepository.SetAsync(user.Id.ToString(), user);

                    break;
                case RequestOperation.Update:
                    var userForUpdate = await _userCacheRepository.GetAsync(user.Id.ToString());

                    if (userForUpdate is null)
                    {
                        userForUpdate = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == user.Id);

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
                    }
                    else
                    {
                        await _userRepository.UpdateAsync(user);
                        await _dialogRepository.UpdateUserAsync(user);
                        await _chatRepository.UpdateUserAsync(user);
                    }

                    await _userCacheRepository.SetAsync(user.Id.ToString(), user);

                    break;
                case RequestOperation.Remove:
                    var userForRemove = await _userCacheRepository.GetAsync(user.Id.ToString());

                    if (userForRemove is null)
                    {
                        userForRemove = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == user.Id);

                        if (userForRemove is not null)
                        {
                            await _userRepository.RemoveAsync(user);
                            await _dialogRepository.RemoveUserAsync(user);
                            await _chatRepository.RemoveUserAsync(user);
                        }
                    }
                    else
                    {
                        await _userRepository.RemoveAsync(user);
                        await _dialogRepository.RemoveUserAsync(user);
                        await _chatRepository.RemoveUserAsync(user);

                        await _userCacheRepository.RemoveAsync(user.Id.ToString());
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
