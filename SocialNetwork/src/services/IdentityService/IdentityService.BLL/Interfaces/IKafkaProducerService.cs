﻿using IdentityService.BLL.DTOs.UserDTOs;

namespace IdentityService.BLL.Interfaces
{
    public interface IKafkaProducerService
    {
        Task SendUserRequestAsync(UserRequest userRequest, GetUserDTO user);
    }
}