namespace ToDo.Domain.Entities;

public class ToDoItem
{
    public int Id { get; init; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int CompletionPercentage { get; set; }
    public DateTimeOffset CreatedOn { get; init; }
    public DateTimeOffset? Expiry { get; set; }

    // factory method for initializing todoitem
    public static ToDoItem Create(string title, string? description = null, DateTimeOffset? expiry = null)
    {
        return new ToDoItem
        {
            Title = title,
            Description = description,
            CompletionPercentage = 0,
            CreatedOn = DateTimeOffset.UtcNow,
            Expiry = expiry
        };
    }
}