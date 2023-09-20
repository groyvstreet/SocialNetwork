using FluentValidation;
using IdentityService.BLL.DTOs.UserDTOs;

namespace IdentityService.BLL.Validators.UserValidators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserDTO>
    {
        public UpdateUserValidator()
        {
            RuleFor(u => u.FirstName).NotEmpty();
            RuleFor(u => u.LastName).NotEmpty();
        }
    }
}
