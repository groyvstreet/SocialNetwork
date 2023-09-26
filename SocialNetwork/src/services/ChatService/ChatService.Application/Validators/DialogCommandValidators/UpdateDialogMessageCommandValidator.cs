using ChatService.Application.Commands.DialogCommands.UpdateDialogMessageCommand;
using FluentValidation;

namespace ChatService.Application.Validators.DialogCommandValidators
{
    public class UpdateDialogMessageCommandValidator : AbstractValidator<UpdateDialogMessageCommand>
    {
        public UpdateDialogMessageCommandValidator()
        {
            RuleFor(c => c.DTO.Text).NotEmpty();
        }
    }
}
