using Ardalis.Result;
using Microsoft.Extensions.Logging;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;

namespace ToDo.Domain.Services;

public class ToDoService(IToDoRepository toDoRepository, ILogger<ToDoService> logger) : IToDoService
{
    public async Task<Result<ToDoItem>> Update(ToDoItem updatedToDoItem)
    {
        var toDoItem = await toDoRepository.GetById(updatedToDoItem.Id);
        if (toDoItem is null)
        {
            logger.LogWarning("Todo item with id: {todoId} was not found.", updatedToDoItem.Id);
            return Result.NotFound();
        }

        toDoItem.Title = updatedToDoItem.Title;
        toDoItem.Description = updatedToDoItem.Description;
        toDoItem.CompletionPercentage = updatedToDoItem.CompletionPercentage;
        toDoItem.Expiry = updatedToDoItem.Expiry;
        await toDoRepository.SaveChangesAsync();

        logger.LogInformation("Successfully updated todo item with id: {todoId}.", updatedToDoItem.Id);
        return toDoItem;
    }

    public async Task<Result<ToDoItem>> SetCompletionPercentage(int itemId, int percentage)
    {
        var toDoItem = await toDoRepository.GetById(itemId);

        if (toDoItem is null)
        {
            logger.LogWarning("Todo item with id: {todoId} was not found.", itemId);
            return Result.NotFound();
        }

        logger.LogInformation(
            "Setting todo item with id: {todoId} completion percentage from {oldPercentage} to: {newPercentage}.",
            itemId, toDoItem.CompletionPercentage, percentage);
        toDoItem.CompletionPercentage = percentage;
        await toDoRepository.SaveChangesAsync();

        return toDoItem;
    }

    public async Task<Result<ToDoItem>> MarkAsDone(int itemId)
    {
        var toDoItem = await toDoRepository.GetById(itemId);
        if (toDoItem is null)
        {
            logger.LogWarning("Todo item with id: {todoId} was not found.", itemId);
            return Result.NotFound();
        }

        toDoItem.CompletionPercentage = 100;
        await toDoRepository.SaveChangesAsync();
        logger.LogInformation("Successfully marked todo item with id: {todoId} as done.", itemId);
        return toDoItem;
    }
}