using System.Net.Http.Json;
using IntegrationTests.Fakes;
using ToDo.API.Contracts.Requests;
using ToDo.API.Contracts.Responses;

namespace IntegrationTests;

public static class HttpClientExtensions
{
    public static async Task<ToDoItemResponse> AddToDoItem(this HttpClient client,
        AddToDoItemRequest? addToDoItemRequest = null)
    {
        if (addToDoItemRequest is null)
        {
            var randomNumber = new Random().Next(1, 12);
            var expiry = DateTimeOffset.UtcNow.AddDays(randomNumber);
            addToDoItemRequest = new AddToDoItemRequestFaker(expiry).Generate();
        }

        var response = await client.PostAsJsonAsync(TestConstants.ToDoApiUri, addToDoItemRequest);
        return (await response.Content.ReadFromJsonAsync<ToDoItemResponse>())!;
    }

    public static async Task<ToDoItemResponse> AddToDoItem(this HttpClient client,
        DateTimeOffset expiryDate)
    {
        var addToDoItemRequest = new AddToDoItemRequestFaker(expiryDate).Generate();
        var response = await client.PostAsJsonAsync(TestConstants.ToDoApiUri, addToDoItemRequest);
        return (await response.Content.ReadFromJsonAsync<ToDoItemResponse>())!;
    }
}