namespace ToDo.API.Contracts.Requests;

public record GetIncomingToDoItemsRequest(DateOnly StartDate, DateOnly? EndDate);