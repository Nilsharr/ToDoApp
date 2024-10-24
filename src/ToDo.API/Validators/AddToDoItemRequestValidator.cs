using FluentValidation;
using ToDo.API.Contracts.Requests;

namespace ToDo.API.Validators;

public class AddToDoItemRequestValidator : AbstractValidator<AddToDoItemRequest>
{
    public AddToDoItemRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.").Length(3, 128)
            .WithMessage("Title must be between 3 and 128 characters.");
        RuleFor(x => x.Description).MaximumLength(512).WithMessage("Description must not exceed 512 characters.");
        RuleFor(x => x.Expiry).GreaterThanOrEqualTo(DateTimeOffset.Now).When(x => x.Expiry.HasValue)
            .WithMessage("Expiry must be a future date.");
    }
}