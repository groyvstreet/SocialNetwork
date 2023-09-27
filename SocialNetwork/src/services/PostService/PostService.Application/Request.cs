using PostService.Domain.Entities;

namespace PostService.Application
{
    public class Request
    {
        public UserRequest Operation { get; set; }

        public User Data { get; set; }
    }
}
