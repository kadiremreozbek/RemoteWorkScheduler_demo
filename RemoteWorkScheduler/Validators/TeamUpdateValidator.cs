using FluentValidation;
using RemoteWorkScheduler.DbContexts;
using RemoteWorkScheduler.Models;

namespace RemoteWorkScheduler.Validators
{
    public class TeamUpdateValidator : AbstractValidator<TeamForUpdateDto>
    {
        private readonly RemoteWorkSchedulerContext _context;

        public TeamUpdateValidator(RemoteWorkSchedulerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            RuleFor(t => t.Id)
                .NotEmpty().WithMessage("Id is required.");

            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name can't be longer than 50 characters.")
                .Must((team, name) => ValidateTeamDoesNotExist(name, team.Id)).WithMessage("Team with this name already exists.");

            RuleFor(t => t.Description)
                .MaximumLength(200).WithMessage("Description can't be longer than 200 characters.");
        }

        private bool ValidateTeamDoesNotExist(string name, Guid teamId)
        {
            return !_context.Teams.Any(c => c.Name == name && c.Id != teamId);
        }
    }
}
