﻿namespace ChatService.Domain.Entities
{
    public class Dialog : IEntity
    {
        public Guid Id { get; set; }

        public ulong MessageCount { get; set; }

        public List<Message> Messages { get; set; }

        public List<User> Users { get; set; }
    }
}
