using FluentValidation;
using ToDo.API.Contracts.Requests;

namespace ToDo.API.Validators;

public class GetIncomingToDoItemsRequestValidator : AbstractValidator<GetIncomingToDoItemsRequest>
{
    public GetIncomingToDoItemsRequestValidator()
    {
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate).When(x => x.EndDate.HasValue)
            .WithMessage("End date must be after the start date.");
    }
}