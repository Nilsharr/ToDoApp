using FluentValidation;
using ToDo.API.Contracts.Requests;

namespace ToDo.API.Validators;

public class GetAllToDoItemsRequestValidator : AbstractValidator<GetAllToDoItemsRequest>
{
    public GetAllToDoItemsRequestValidator()
    {
        RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(0).WithMessage("Page index must be 0 or greater.");
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}