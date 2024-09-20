using FluentValidation;
using RemoteWorkScheduler.Models;

namespace RemoteWorkScheduler.Validators
{
    public class TeamCreationValidator : AbstractValidator<TeamForCreationDto>
    {
        public TeamCreationValidator()
        {
            RuleFor(team => team.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name can't be longer than 50 characters.");
        }
    }
}
