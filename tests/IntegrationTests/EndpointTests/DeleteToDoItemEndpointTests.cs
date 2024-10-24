using System.Net;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ToDo.Domain.Interfaces;

namespace IntegrationTests.EndpointTests;

[Collection(ToDoApiCollection.CollectionName)]
public class DeleteToDoItemEndpointTests(ToDoApiFactory toDoApiFactory) : IAsyncLifetime
{
    private readonly HttpClient _client = toDoApiFactory.HttpClient;

    private readonly IToDoRepository _toDoRepository =
        toDoApiFactory.Services.CreateScope().ServiceProvider.GetRequiredService<IToDoRepository>();

    private readonly Func<Task> _resetDatabase = toDoApiFactory.ResetDatabase;

    [Fact]
    public async Task ToDoItemExists_ShouldDeleteToDoItem()
    {
        var createdToDoItem = await _client.AddToDoItem();

        var response = await _client.DeleteAsync(TestConstants.ToDoApiUri + $"/{createdToDoItem.Id}");
        var toDoItem = await _toDoRepository.GetById(createdToDoItem.Id);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        toDoItem.Should().BeNull();
    }

    [Fact]
    public async Task ToDoItemDoesNotExist_ShouldReturnNotFound()
    {
        const int notExistingId = 1;

        var response = await _client.DeleteAsync(TestConstants.ToDoApiUri + $"/{notExistingId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}