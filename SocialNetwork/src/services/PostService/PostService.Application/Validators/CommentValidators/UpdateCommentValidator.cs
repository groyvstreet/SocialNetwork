using FluentValidation;
using PostService.Application.DTOs.CommentDTOs;

namespace PostService.Application.Validators.CommentValidators
{
    public class UpdateCommentValidator : AbstractValidator<UpdateCommentDTO>
    {
        public UpdateCommentValidator()
        {
            RuleFor(c => c.Text).NotEmpty();
        }
    }
}
