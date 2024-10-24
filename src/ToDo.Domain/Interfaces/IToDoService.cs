using Ardalis.Result;
using ToDo.Domain.Entities;

namespace ToDo.Domain.Interfaces;

public interface IToDoService
{
    Task<Result<ToDoItem>> Update(ToDoItem toDoItem);
    Task<Result<ToDoItem>> SetCompletionPercentage(int itemId, int percentage);
    Task<Result<ToDoItem>> MarkAsDone(int itemId);
}