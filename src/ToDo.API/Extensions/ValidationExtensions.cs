using ToDo.API.Filters;

namespace ToDo.API.Extensions;

public static class ValidationExtensions
{
    // add validation filter for endpoint
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<ValidationFilter<TRequest>>().ProducesValidationProblem();
    }
}