using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ToDo.Domain.Interfaces;

namespace IntegrationTests.EndpointTests;

[Collection(ToDoApiCollection.CollectionName)]
public class MarkToDoItemAsDoneEndpointTests(ToDoApiFactory toDoApiFactory) : IAsyncLifetime
{
    private readonly HttpClient _client = toDoApiFactory.HttpClient;

    private readonly IToDoRepository _toDoRepository =
        toDoApiFactory.Services.CreateScope().ServiceProvider.GetRequiredService<IToDoRepository>();

    private readonly Func<Task> _resetDatabase = toDoApiFactory.ResetDatabase;

    [Fact]
    public async Task ToDoItemExists_ShouldMarkToDoItemAsDone()
    {
        var addToDoItem = await _client.AddToDoItem();
        const int expectedCompletionPercentage = 100;

        var response = await _client.PatchAsync(TestConstants.ToDoApiUri + $"/{addToDoItem.Id}/done", null);
        var toDoItem = await _toDoRepository.GetById(addToDoItem.Id);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        toDoItem!.CompletionPercentage.Should().Be(expectedCompletionPercentage);
    }

    [Fact]
    public async Task ToDoItemDoesNotExist_ShouldReturnNotFound()
    {
        const int notExistingId = 1;

        var response = await _client.PatchAsync(TestConstants.ToDoApiUri + $"/{notExistingId}/done", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}