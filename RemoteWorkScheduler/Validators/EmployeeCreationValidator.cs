using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RemoteWorkScheduler.DbContexts;
using RemoteWorkScheduler.Models;
using RemoteWorkScheduler.Services;

namespace RemoteWorkScheduler.Validators
{
    public class EmployeeCreationValidator : AbstractValidator<EmployeeForCreationDto>
    {
        public EmployeeCreationValidator()

        {
            RuleFor(e => e.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50);

            RuleFor(e => e.Title)
                .IsInEnum().WithMessage("Title must be in job list.");

            RuleFor(e => e.TeamId)
                .NotEmpty().WithMessage("TeamId is required.");
                //.Must(ValidateTeamIdExists).WithMessage("TeamId does not exist.");
        }

        /*private bool ValidateTeamIdExists(Guid teamId)
        {
            return _context.Teams.Any(c => c.Id == teamId);
        }*/
    }
}
