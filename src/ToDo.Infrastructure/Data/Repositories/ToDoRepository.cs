using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Domain.Models;

namespace ToDo.Infrastructure.Data.Repositories;

public class ToDoRepository(ToDoDbContext dbContext) : IToDoRepository
{
    public async Task<PaginatedList<ToDoItem>> GetAll(int pageIndex, int pageSize, bool showExpired)
    {
        var query = dbContext.ToDoItems.AsNoTracking().AsQueryable();
        // show only to do items that are not expired or do not have expiration date
        if (!showExpired)
        {
            query = query.Where(x => x.Expiry == null || x.Expiry > DateTimeOffset.UtcNow);
        }

        var totalCount = await query.CountAsync();
        if (totalCount == 0)
        {
            return new PaginatedList<ToDoItem>(pageIndex, pageSize, 0, []);
        }

        var skip = pageIndex * pageSize;
        var items = await query.OrderBy(x => x.CreatedOn).Skip(skip).Take(pageSize).ToListAsync();

        return new PaginatedList<ToDoItem>(pageIndex, pageSize, totalCount, items);
    }

    public ValueTask<ToDoItem?> GetById(int id)
    {
        return dbContext.ToDoItems.FindAsync(id);
    }

    public async Task<IReadOnlyCollection<ToDoItem>> GetIncoming(DateOnly startDate, DateOnly? endDate)
    {
        var query = dbContext.ToDoItems.AsNoTracking().AsQueryable().OrderBy(x => x.Expiry);
        // when startDate and endDate are defined, return todoitems with expiry within a given period
        if (endDate.HasValue)
        {
            return await query.Where(x =>
                DateOnly.FromDateTime(x.Expiry!.Value.Date) >= startDate &&
                DateOnly.FromDateTime(x.Expiry!.Value.Date) <= endDate).ToListAsync();
        }

        // when only startDate is defined, return todoitems with expiry on a given day
        return await query.Where(x => DateOnly.FromDateTime(x.Expiry!.Value.Date) == startDate).ToListAsync();
    }

    public Task<DateTimeOffset?> GetExpiryDate(int id)
    {
        return dbContext.ToDoItems.Where(x => x.Id == id).Select(x => x.Expiry).SingleOrDefaultAsync();
    }

    public void Add(ToDoItem toDoItem)
    {
        dbContext.ToDoItems.Add(toDoItem);
    }

    public Task<int> ExecuteDelete(int id)
    {
        return dbContext.ToDoItems.Where(x => x.Id == id).ExecuteDeleteAsync();
    }

    public Task<int> SaveChangesAsync()
    {
        return dbContext.SaveChangesAsync();
    }
}