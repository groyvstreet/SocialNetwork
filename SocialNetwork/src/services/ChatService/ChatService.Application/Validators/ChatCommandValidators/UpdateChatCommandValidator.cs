using ChatService.Application.Commands.ChatCommands.UpdateChatCommand;
using FluentValidation;

namespace ChatService.Application.Validators.ChatCommandValidators
{
    public class UpdateChatCommandValidator : AbstractValidator<UpdateChatCommand>
    {
        public UpdateChatCommandValidator()
        {
            RuleFor(c => c.DTO.Name).NotEmpty();
            RuleFor(c => c.DTO.Image).NotEmpty();
        }
    }
}
