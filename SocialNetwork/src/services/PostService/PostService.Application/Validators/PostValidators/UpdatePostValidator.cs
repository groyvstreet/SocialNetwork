using FluentValidation;
using PostService.Application.DTOs.PostDTOs;

namespace PostService.Application.Validators.PostValidators
{
    public class UpdatePostValidator : AbstractValidator<UpdatePostDTO>
    {
        public UpdatePostValidator()
        {
            RuleFor(p => p.Text).NotEmpty();
        }
    }
}
