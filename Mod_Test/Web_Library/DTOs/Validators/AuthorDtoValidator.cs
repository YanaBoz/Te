using FluentValidation;
using Web_Library.DTOs;

namespace Web_Library.Validators
{
    public class AuthorDtoValidator : AbstractValidator<AuthorDto>
    {
        public AuthorDtoValidator()
        {
            RuleFor(a => a.FirstName)
                .NotEmpty().WithMessage("FirstName is required")
                .MaximumLength(50).WithMessage("FirstName cannot exceed 50 characters");

            RuleFor(a => a.LastName)
                .NotEmpty().WithMessage("LastName is required")
                .MaximumLength(50).WithMessage("LastName cannot exceed 50 characters");

            RuleFor(a => a.BirthDate)
                .LessThan(DateTime.Now).WithMessage("BirthDate must be in the past");

            RuleFor(a => a.Country)
                .NotEmpty().WithMessage("Country is required")
                .MaximumLength(50).WithMessage("Country cannot exceed 50 characters");
        }
    }
}
