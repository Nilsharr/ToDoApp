using ToDo.API.Contracts.Requests;
using ToDo.API.Contracts.Responses;
using ToDo.Domain.Entities;
using ToDo.Domain.Models;

namespace ToDo.API.Mapping;

public static class ToDoItemMapper
{
    public static ToDoItemResponse FromEntity(ToDoItem toDoItem) => new(toDoItem.Id, toDoItem.Title,
        toDoItem.CompletionPercentage, toDoItem.CreatedOn, toDoItem.Description, toDoItem.Expiry);

    public static IEnumerable<ToDoItemResponse> FromEntities(IEnumerable<ToDoItem> toDoItems) =>
        toDoItems.Select(FromEntity);

    public static PaginatedList<ToDoItemResponse> FromEntities(PaginatedList<ToDoItem> paginatedList) => new(
        paginatedList.PageIndex, paginatedList.PageSize, paginatedList.TotalCount,
        FromEntities(paginatedList.Data).ToList());

    public static ToDoItem ToEntity(AddToDoItemRequest req) => ToDoItem.Create(req.Title, req.Description, req.Expiry);

    public static ToDoItem ToEntity(UpdateToDoItemRequest req) => new()
    {
        Id = req.Id, Title = req.Title, CompletionPercentage = req.CompletionPercentage, Description = req.Description,
        Expiry = req.Expiry
    };
}