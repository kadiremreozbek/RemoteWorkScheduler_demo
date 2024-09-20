using FluentValidation.Results;

namespace RemoteWorkScheduler.Validators
{
    public interface IValidator<T>
    {
        ValidationResult Validate(T instance);
        Task<ValidationResult> ValidateAsync(T instance);

    }
}
