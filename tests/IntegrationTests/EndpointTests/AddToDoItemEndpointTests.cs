using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IntegrationTests.Fakes;
using ToDo.API.Contracts.Requests;
using ToDo.API.Contracts.Responses;

namespace IntegrationTests.EndpointTests;

[Collection(ToDoApiCollection.CollectionName)]
public class AddToDoItemEndpointTests(ToDoApiFactory toDoApiFactory) : IAsyncLifetime
{
    private readonly HttpClient _client = toDoApiFactory.HttpClient;
    private readonly Func<Task> _resetDatabase = toDoApiFactory.ResetDatabase;

    [Fact]
    public async Task RequestIsValid_ShouldCreateToDoItem()
    {
        var addToDoItemRequest = new AddToDoItemRequestFaker(DateTimeOffset.UtcNow.AddDays(3)).Generate();

        var response = await _client.PostAsJsonAsync(TestConstants.ToDoApiUri, addToDoItemRequest);
        var toDoItemResponse = await response.Content.ReadFromJsonAsync<ToDoItemResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        toDoItemResponse.Should().NotBeNull();
        response.Headers.Location.Should().Be($"http://localhost/{TestConstants.ToDoApiUri}/{toDoItemResponse!.Id}");
        toDoItemResponse.Id.Should().BePositive();
        toDoItemResponse.Title.Should().Be(addToDoItemRequest.Title);
        toDoItemResponse.Description.Should().Be(addToDoItemRequest.Description);
        toDoItemResponse.CompletionPercentage.Should().Be(0);
        toDoItemResponse.Expiry.Should().Be(addToDoItemRequest.Expiry);
        toDoItemResponse.CreatedOn.Should().NotBe(DateTimeOffset.MinValue);
    }

    [Fact]
    public async Task RequestIsInvalid_ShouldReturnBadRequest()
    {
        var addToDoItemRequest = new AddToDoItemRequest("");

        var response = await _client.PostAsJsonAsync(TestConstants.ToDoApiUri, addToDoItemRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}