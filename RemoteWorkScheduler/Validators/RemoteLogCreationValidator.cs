using FluentValidation;
using RemoteWorkScheduler.Models;
using RemoteWorkScheduler.DbContexts;


namespace RemoteWorkScheduler.Validators
{
    public class RemoteLogCreationValidator : AbstractValidator<RemoteLogForCreationDto>
    {
        private readonly RemoteWorkSchedulerContext _context;

        public RemoteLogCreationValidator(RemoteWorkSchedulerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            RuleFor(remoteLog => remoteLog.Date)
                .NotEmpty().WithMessage("Date is required.")
                .Must(date => date.Date >= DateTime.Today).WithMessage("Date must be in the future.");
            RuleFor(remoteLog => remoteLog.EmployeeId)
                .NotEmpty().WithMessage("EmployeeId is required.")
                .Must(EmployeeExists).WithMessage("EmployeeId does not exist.");
        }

        public bool EmployeeExists(Guid employeeId)
        {
            return  _context.Employees.Any(c => c.Id == employeeId);
        }
    }
}
