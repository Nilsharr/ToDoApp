using FluentValidation;
using ToDo.API.Contracts.Requests;

namespace ToDo.API.Validators;

public class SetToDoItemCompletionPercentageRequestValidator : AbstractValidator<SetToDoItemCompletionPercentageRequest>
{
    public SetToDoItemCompletionPercentageRequestValidator()
    {
        RuleFor(x => x.Percentage).InclusiveBetween(0, 100).WithMessage("Percentage must be between 0 and 100.");
    }
}