namespace IdentityService.BLL.Interfaces
{
    public interface IKafkaProducerService<TOperation, TData>
    {
        Task SendUserRequestAsync(TOperation operationRequest, TData user);
    }
}
