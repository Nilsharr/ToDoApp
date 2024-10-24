using Ardalis.Result;
using Microsoft.AspNetCore.Http.HttpResults;
using ToDo.API.Contracts.Requests;
using ToDo.API.Contracts.Responses;
using ToDo.API.Extensions;
using ToDo.API.Mapping;
using ToDo.Domain.Interfaces;
using ToDo.Domain.Models;

namespace ToDo.API.Endpoints;

public static class ToDoEndpoints
{
    private const string GetToDoItemEndpointName = "GetToDoItem";

    public static IEndpointRouteBuilder MapToDoEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/todoitems").WithTags("ToDoEndpoints");

        api.MapGet("", GetAllToDoItems).WithRequestValidation<GetAllToDoItemsRequest>()
            .WithSummary("Get all todo items.")
            .WithDescription(
                "If expired parameter is set to true returns all todo items, else only those items that are not expired or do not have expiration date set.");
        api.MapGet("/incoming", GetIncomingToDoItems).WithRequestValidation<GetIncomingToDoItemsRequest>()
            .WithSummary("Get incoming todo items.")
            .WithDescription(
                $"""
                 Start date - the beginning date of the period to search for expiring todo items. Format: "yyyy-MM-dd" (e.g., "2024-10-22"). {Environment.NewLine} 
                 End date - the last date of the period to search for expiring todo items. Format: "yyyy-MM-dd" (e.g., "2024-10-22"). {Environment.NewLine} 
                 If only the start date is set the endpoint will return expiring todo items from that day.
                 """);
        api.MapGet("/{id:int}", GetToDoItem).WithName(GetToDoItemEndpointName).WithSummary("Get todo item.");

        api.MapPost("", AddToDoItem).WithRequestValidation<AddToDoItemRequest>().WithSummary("Add todo item.");
        api.MapPut("", UpdateToDoItem).WithRequestValidation<UpdateToDoItemRequest>()
            .WithSummary("Update todo item.");
        api.MapPatch("/{id:int}/percentage", SetToDoItemCompletionPercentage)
            .WithRequestValidation<SetToDoItemCompletionPercentageRequest>()
            .WithSummary("Set todo item completion percentage.");
        api.MapPatch("/{id:int}/done", MarkToDoItemAsDone).WithSummary("Mark todo item as done.");
        api.MapDelete("/{id:int}", DeleteToDoItem).WithSummary("Delete todo item.");

        return api;
    }

    public static async Task<Ok<PaginatedList<ToDoItemResponse>>> GetAllToDoItems(
        [AsParameters] GetAllToDoItemsRequest toDoItemsRequest, IToDoRepository toDoRepository)
    {
        var items = await toDoRepository.GetAll(toDoItemsRequest.PageIndex, toDoItemsRequest.PageSize,
            toDoItemsRequest.Expired);

        return TypedResults.Ok(ToDoItemMapper.FromEntities(items));
    }

    public static async Task<Ok<IEnumerable<ToDoItemResponse>>> GetIncomingToDoItems(
        [AsParameters] GetIncomingToDoItemsRequest toDoItemsRequest, IToDoRepository toDoRepository)
    {
        var toDoItems = await toDoRepository.GetIncoming(toDoItemsRequest.StartDate, toDoItemsRequest.EndDate);

        return TypedResults.Ok(ToDoItemMapper.FromEntities(toDoItems));
    }

    public static async Task<Results<Ok<ToDoItemResponse>, NotFound>> GetToDoItem(int id,
        IToDoRepository toDoRepository)
    {
        var toDoItem = await toDoRepository.GetById(id);

        return toDoItem is null ? TypedResults.NotFound() : TypedResults.Ok(ToDoItemMapper.FromEntity(toDoItem));
    }

    public static async Task<CreatedAtRoute<ToDoItemResponse>> AddToDoItem(AddToDoItemRequest toDoItemRequest,
        IToDoRepository toDoRepository)
    {
        var created = ToDoItemMapper.ToEntity(toDoItemRequest);
        toDoRepository.Add(created);
        await toDoRepository.SaveChangesAsync();
        return TypedResults.CreatedAtRoute(ToDoItemMapper.FromEntity(created), GetToDoItemEndpointName,
            new { created.Id });
    }

    public static async Task<Results<Ok<ToDoItemResponse>, NotFound>> UpdateToDoItem(
        UpdateToDoItemRequest toDoItemRequest, IToDoService toDoService)
    {
        var result = await toDoService.Update(ToDoItemMapper.ToEntity(toDoItemRequest));
        if (result.IsNotFound())
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(ToDoItemMapper.FromEntity(result.Value));
    }

    public static async Task<Results<NoContent, NotFound>> SetToDoItemCompletionPercentage(int id,
        SetToDoItemCompletionPercentageRequest request, IToDoService toDoService)
    {
        var result = await toDoService.SetCompletionPercentage(id, request.Percentage);

        if (result.IsNotFound())
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound>> MarkToDoItemAsDone(int id, IToDoService toDoService)
    {
        var result = await toDoService.MarkAsDone(id);

        if (result.IsNotFound())
        {
            return TypedResults.NotFound();
        }

        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound>> DeleteToDoItem(int id, IToDoRepository toDoRepository)
    {
        var deleted = await toDoRepository.ExecuteDelete(id);
        return deleted > 0 ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}