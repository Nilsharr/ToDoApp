using FluentValidation;

namespace ToDo.API.Filters;

internal class ValidationFilter<TRequest>(IValidator<TRequest> validator, ILogger<ValidationFilter<TRequest>> logger)
    : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // validate incoming request using fluent validation
        var request = context.Arguments.OfType<TRequest>().First();

        logger.LogInformation("Validating request: {@Request}", request);
        var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);

        if (result.IsValid)
        {
            return await next(context);
        }

        logger.LogWarning("Invalid request: {@error}", result.ToDictionary());
        return TypedResults.ValidationProblem(result.ToDictionary());
    }
}