namespace ToDo.API.Contracts.Requests;

public record UpdateToDoItemRequest(
    int Id,
    string Title,
    int CompletionPercentage,
    string? Description = null,
    DateTimeOffset? Expiry = null);