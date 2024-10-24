using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ToDo.API.Contracts.Requests;
using ToDo.Domain.Interfaces;

namespace IntegrationTests.EndpointTests;

[Collection(ToDoApiCollection.CollectionName)]
public class SetToDoItemCompletionPercentageEndpointTests(ToDoApiFactory toDoApiFactory) : IAsyncLifetime
{
    private readonly HttpClient _client = toDoApiFactory.HttpClient;

    private readonly IToDoRepository _toDoRepository =
        toDoApiFactory.Services.CreateScope().ServiceProvider.GetRequiredService<IToDoRepository>();

    private readonly Func<Task> _resetDatabase = toDoApiFactory.ResetDatabase;

    [Fact]
    public async Task ToDoItemExists_ShouldSetToDoItemCompletionPercentage()
    {
        var addToDoItem = await _client.AddToDoItem();
        var setToDoItemCompletionPercentageRequest = new SetToDoItemCompletionPercentageRequest(50);

        var response = await _client.PatchAsJsonAsync(TestConstants.ToDoApiUri + $"/{addToDoItem.Id}/percentage",
            setToDoItemCompletionPercentageRequest);
        var toDoItem = await _toDoRepository.GetById(addToDoItem.Id);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        toDoItem!.CompletionPercentage.Should().Be(setToDoItemCompletionPercentageRequest.Percentage);
    }

    [Fact]
    public async Task ToDoItemDoesNotExist_ShouldReturnNotFound()
    {
        const int notExistingId = 1;
        var setToDoItemCompletionPercentageRequest = new SetToDoItemCompletionPercentageRequest(75);

        var response = await _client.PatchAsJsonAsync(TestConstants.ToDoApiUri + $"/{notExistingId}/percentage",
            setToDoItemCompletionPercentageRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}