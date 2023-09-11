using FluentValidation;
using IdentityService.BLL.DTOs.UserDTOs;

namespace IdentityService.BLL.Validators.UserValidators
{
    public class AddUserValidator : AbstractValidator<AddUserDTO>
    {
        public AddUserValidator()
        {
            RuleFor(u => u.FirstName).NotEmpty();
            RuleFor(u => u.LastName).NotEmpty();
        }
    }
}
