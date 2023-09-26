using ChatService.Application.Commands.ChatCommands.AddChatCommand;
using FluentValidation;

namespace ChatService.Application.Validators.ChatCommandValidators
{
    public class AddChatCommandValidator : AbstractValidator<AddChatCommand>
    {
        public AddChatCommandValidator()
        {
            RuleFor(c => c.DTO.Name).NotEmpty();
        }
    }
}
