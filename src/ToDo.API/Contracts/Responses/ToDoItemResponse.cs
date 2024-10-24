using System.Diagnostics.CodeAnalysis;

namespace ToDo.API.Contracts.Responses;

[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global")]
public record ToDoItemResponse(
    int Id,
    string Title,
    int CompletionPercentage,
    DateTimeOffset CreatedOn,
    string? Description = null,
    DateTimeOffset? Expiry = null)
{
    public bool IsExpired => Expiry < DateTimeOffset.Now;
    public bool IsDone => CompletionPercentage == 100;
}