﻿namespace IdentityService.BL.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool IsEmailConfirmed { get; set; }
    }
}
