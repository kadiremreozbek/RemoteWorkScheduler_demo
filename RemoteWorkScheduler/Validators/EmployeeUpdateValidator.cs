using FluentValidation;
using RemoteWorkScheduler.DbContexts;
using RemoteWorkScheduler.Models;

namespace RemoteWorkScheduler.Validators
{
    public class EmployeeUpdateValidator : AbstractValidator<EmployeeForUpdateDto>
    {
        private readonly RemoteWorkSchedulerContext _context;

        public EmployeeUpdateValidator(RemoteWorkSchedulerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            RuleFor(e => e.Id)
                .NotEmpty().WithMessage("Id is required.");
                //.Must(ValidateEmployeeIdIsSame).WithMessage("EmployeeId does not match the provided Id.");

            RuleFor(e => e.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50);

            RuleFor(e => e.Title)
                .IsInEnum().WithMessage("Title must be in job list.");

            RuleFor(e => e.TeamId)
                .NotEmpty().WithMessage("TeamId is required.")
                .Must(ValidateTeamIdExists).WithMessage("TeamId does not exist.");
        }

        private bool ValidateTeamIdExists(Guid teamId)
        {
            return _context.Teams.Any(c => c.Id == teamId);
        }

        /*private bool ValidateEmployeeIdIsSame(Guid id)
        {
            return id == _employeeId;
        }*/
    }
}
