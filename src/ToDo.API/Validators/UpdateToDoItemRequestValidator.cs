using FluentValidation;
using ToDo.API.Contracts.Requests;
using ToDo.Domain.Interfaces;

namespace ToDo.API.Validators;

public class UpdateToDoItemRequestValidator : AbstractValidator<UpdateToDoItemRequest>
{
    public UpdateToDoItemRequestValidator(IToDoRepository toDoRepository)
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.").Length(3, 128)
            .WithMessage("Title must be between 3 and 128 characters.");
        RuleFor(x => x.Description).MaximumLength(512).WithMessage("Description must not exceed 512 characters.");
        RuleFor(x => x.CompletionPercentage).InclusiveBetween(0, 100)
            .WithMessage("Percentage must be between 0 and 100.");

        RuleFor(x => x.Expiry).GreaterThanOrEqualTo(DateTimeOffset.Now).When(x => x.Expiry.HasValue)
            // assuming it's possible to update expired items, don't validate expiry date if it hasn't been changed
            .WhenAsync(async (x, _) => x.Expiry != await toDoRepository.GetExpiryDate(x.Id))
            .WithMessage("Expiry must be a future date.");
    }
}