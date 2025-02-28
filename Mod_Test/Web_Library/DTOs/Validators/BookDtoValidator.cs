using FluentValidation;
using Web_Library.DTOs;

namespace Web_Library.Validators
{
    public class BookDtoValidator : AbstractValidator<BookDto>
    {
        public BookDtoValidator()
        {
            RuleFor(b => b.ISBN)
                .NotEmpty().WithMessage("ISBN is required")
                .Length(13).WithMessage("ISBN must be exactly 13 characters");

            RuleFor(b => b.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(b => b.Genre)
                .NotEmpty().WithMessage("Genre is required")
                .MaximumLength(100).WithMessage("Genre cannot exceed 100 characters");

            RuleFor(b => b.Description)
                .NotNull().WithMessage("Description must be provided");

            RuleFor(b => b.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative");
        }
    }
}
