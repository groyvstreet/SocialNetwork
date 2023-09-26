using ChatService.Application.Commands.ChatCommands.AddChatMessageCommand;
using FluentValidation;

namespace ChatService.Application.Validators.ChatCommandValidators
{
    public class AddChatMessageCommandValidator : AbstractValidator<AddChatMessageCommand>
    {
        public AddChatMessageCommandValidator()
        {
            RuleFor(c => c.DTO.Text).NotEmpty();
        }
    }
}
