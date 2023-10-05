﻿namespace PostService.Infrastructure.Interfaces
{
    public interface IKafkaConsumerHandler<TOperation, TData>
    {
        Task HandleAsync(TOperation operation, TData data);
    }
}
