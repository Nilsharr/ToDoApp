namespace ToDo.API.Contracts.Requests;

public record AddToDoItemRequest(
    string Title,
    string? Description = null,
    DateTimeOffset? Expiry = null
);