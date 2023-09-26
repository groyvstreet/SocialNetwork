using ChatService.Application.Commands.ChatCommands.UpdateChatMessageCommand;
using FluentValidation;

namespace ChatService.Application.Validators.ChatCommandValidators
{
    public class UpdateChatMessageCommandValidator : AbstractValidator<UpdateChatMessageCommand>
    {
        public UpdateChatMessageCommandValidator()
        {
            RuleFor(c => c.DTO.Text).NotEmpty();
        }
    }
}
