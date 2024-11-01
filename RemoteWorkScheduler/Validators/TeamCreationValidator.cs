﻿using FluentValidation;
using RemoteWorkScheduler.DbContexts;
using RemoteWorkScheduler.Models;

namespace RemoteWorkScheduler.Validators
{
    public class TeamCreationValidator : AbstractValidator<TeamForCreationDto>
    {
        private readonly RemoteWorkSchedulerContext _context;

        public TeamCreationValidator(RemoteWorkSchedulerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            RuleFor(team => team.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name can't be longer than 50 characters.")
                .Must(ValidateTeamDoesNotExist).WithMessage("Team with this name already exists.");

            RuleFor(team => team.Description)
                .MaximumLength(200).WithMessage("Description can't be longer than 200 characters.");
        }

        private bool ValidateTeamDoesNotExist(string name)
        {
            return !_context.Teams.Any(c => c.Name == name);
        }
    }
}
