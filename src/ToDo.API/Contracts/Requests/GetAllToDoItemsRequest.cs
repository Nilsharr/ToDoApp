namespace ToDo.API.Contracts.Requests;

public record GetAllToDoItemsRequest(int PageIndex = 0, int PageSize = 10, bool Expired = true);