using PostService.Application;
using PostService.Application.Interfaces.UserInterfaces;
using PostService.Domain.Entities;
using PostService.Infrastructure.Interfaces;

namespace PostService.Infrastructure.Services
{
    public class UserKafkaConsumerHandler : IKafkaConsumerHandler<RequestOperation, User>
    {
        private readonly IUserRepository _userRepository;

        public UserKafkaConsumerHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
                        _userRepository.Update(userForUpdate);
                    }

                    break;
                case RequestOperation.Remove:
                    var userForRemove = await _userRepository.GetFirstOrDefaultByAsync(u => u.Id == user.Id);

                    if (userForRemove is not null)
                    {
                        _userRepository.Remove(userForRemove);
                    }

                    break;
                default:
                    break;
            }

            await _userRepository.SaveChangesAsync();
        }
    }
}
