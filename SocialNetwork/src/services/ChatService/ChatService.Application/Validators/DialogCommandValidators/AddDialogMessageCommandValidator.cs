using ChatService.Application.Commands.DialogCommands.AddDialogMessageCommand;
using FluentValidation;

namespace ChatService.Application.Validators.DialogCommandValidators
{
    public class AddDialogMessageCommandValidator : AbstractValidator<AddDialogMessageCommand>
    {
        public AddDialogMessageCommandValidator()
        {
            RuleFor(c => c.DTO.Text).NotEmpty();
            RuleFor(c => c.DateTime).GreaterThan(DateTimeOffset.Now);
        }
    }
}
