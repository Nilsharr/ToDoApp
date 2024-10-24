using FluentValidation;
using ToDo.API.Contracts.Requests;
using ToDo.API.Validators;

namespace ToDo.API.Extensions;

public static class ApiServiceExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<GetAllToDoItemsRequest>, GetAllToDoItemsRequestValidator>();
        services.AddScoped<IValidator<GetIncomingToDoItemsRequest>, GetIncomingToDoItemsRequestValidator>();
        services.AddScoped<IValidator<AddToDoItemRequest>, AddToDoItemRequestValidator>();
        services.AddScoped<IValidator<UpdateToDoItemRequest>, UpdateToDoItemRequestValidator>();
        services
            .AddScoped<IValidator<SetToDoItemCompletionPercentageRequest>,
                SetToDoItemCompletionPercentageRequestValidator>();

        return services;
    }
}