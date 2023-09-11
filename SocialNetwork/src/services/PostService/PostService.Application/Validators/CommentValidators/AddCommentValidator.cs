using FluentValidation;
using PostService.Application.DTOs.CommentDTOs;

namespace PostService.Application.Validators.CommentValidators
{
    public class AddCommentValidator : AbstractValidator<AddCommentDTO>
    {
        public AddCommentValidator()
        {
            RuleFor(c => c.Text).NotEmpty();
        }
    }
}
