using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Extensions;
using IntegrationTests.Fakes;
using ToDo.API.Contracts.Requests;
using ToDo.API.Contracts.Responses;

namespace IntegrationTests.EndpointTests;

[Collection(ToDoApiCollection.CollectionName)]
public class UpdateToDoItemEndpointTests(ToDoApiFactory toDoApiFactory) : IAsyncLifetime
{
    private readonly HttpClient _client = toDoApiFactory.HttpClient;
    private readonly Func<Task> _resetDatabase = toDoApiFactory.ResetDatabase;

    [Fact]
    public async Task RequestIsValid_ShouldUpdateToDoItem()
    {
        var createdToDoItem = await _client.AddToDoItem();
        var updateToDoItemRequest =
            new UpdateToDoItemRequestFaker(createdToDoItem.Id, DateTimeOffset.UtcNow.AddDays(5)).Generate();

        var response = await _client.PutAsJsonAsync(TestConstants.ToDoApiUri, updateToDoItemRequest);
        var toDoItemResponse = await response.Content.ReadFromJsonAsync<ToDoItemResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        toDoItemResponse.Should().NotBeNull();
        toDoItemResponse!.Id.Should().Be(updateToDoItemRequest.Id);
        toDoItemResponse.Title.Should().Be(updateToDoItemRequest.Title);
        toDoItemResponse.Description.Should().Be(updateToDoItemRequest.Description);
        toDoItemResponse.CompletionPercentage.Should().Be(updateToDoItemRequest.CompletionPercentage);
        toDoItemResponse.Expiry.Should().Be(updateToDoItemRequest.Expiry);
        toDoItemResponse.CreatedOn.Should().BeCloseTo(createdToDoItem.CreatedOn, 10.Milliseconds());
    }

    [Fact]
    public async Task RequestIsInvalid_ShouldReturnBadRequest()
    {
        var createdToDoItem = await _client.AddToDoItem();

        var updateToDoItemRequest = new UpdateToDoItemRequest(createdToDoItem.Id, "", -10);

        var response = await _client.PutAsJsonAsync(TestConstants.ToDoApiUri, updateToDoItemRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ToDoItemDoesNotExist_ShouldReturnNotFound()
    {
        var updateToDoItemRequest = new UpdateToDoItemRequestFaker(1).Generate();

        var response = await _client.PutAsJsonAsync(TestConstants.ToDoApiUri, updateToDoItemRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}