using FluentValidation;
using PostService.Application.DTOs.PostDTOs;

namespace PostService.Application.Validators.PostValidators
{
    public class AddPostValidator : AbstractValidator<AddPostDTO>
    {
        public AddPostValidator()
        {
            RuleFor(p => p.Text).NotEmpty();
        }
    }
}
