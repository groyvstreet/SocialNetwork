using PostService.Application;
using PostService.Application.Interfaces;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;
using PostService.Infrastructure.Interfaces;

namespace PostService.Infrastructure.Services
{
    public class UserKafkaConsumerHandler : IKafkaConsumerHandler<RequestOperation, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheRepository<User> _userCacheRepository;

        public UserKafkaConsumerHandler(IUserRepository userRepository,
                                        ICacheRepository<User> userCacheRepository)
        {
            _userRepository = userRepository;
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
                        userForUpdate = await _userRepository.GetFirstOrDefaultAsNoTrackingByAsync(u => u.Id == user.Id);

                        if (userForUpdate is null)
                        {
                            await _userRepository.AddAsync(user);
                        }
                        else
                        {
                            _userRepository.Update(user);
                        }
                    }
                    else
                    {
                        _userRepository.Update(user);
                    }

                    await _userCacheRepository.SetAsync(user.Id.ToString(), user);
                    break;
                case RequestOperation.Remove:
                    var userForRemove = await _userCacheRepository.GetAsync(user.Id.ToString());

                    if (userForRemove is null)
                    {
                        userForRemove = await _userRepository.GetFirstOrDefaultAsNoTrackingByAsync(u => u.Id == user.Id);

                        if (userForRemove is not null)
                        {
                            _userRepository.Remove(userForRemove);
                        }
                    }
                    else
                    {
                        _userRepository.Remove(userForRemove);

                        await _userCacheRepository.RemoveAsync(user.Id.ToString());
                    }

                    break;
                default:
                    break;
            }

            await _userRepository.SaveChangesAsync();
            _userRepository.ClearTrackedEntities();
        }
    }
}
