using ToDo.Domain.Entities;
using ToDo.Domain.Models;

namespace ToDo.Domain.Interfaces;

public interface IToDoRepository
{
    Task<PaginatedList<ToDoItem>> GetAll(int pageIndex, int pageSize, bool showExpired);
    ValueTask<ToDoItem?> GetById(int id);
    Task<IReadOnlyCollection<ToDoItem>> GetIncoming(DateOnly startDate, DateOnly? endDate);
    Task<DateTimeOffset?> GetExpiryDate(int id);
    void Add(ToDoItem toDoItem);
    Task<int> ExecuteDelete(int id);
    Task<int> SaveChangesAsync();
}