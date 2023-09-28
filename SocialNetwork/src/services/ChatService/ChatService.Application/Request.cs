namespace ChatService.Application
{
    public class Request<T>
    {
        public RequestOperation Operation { get; set; }

        public T Data { get; set; }
    }
}
