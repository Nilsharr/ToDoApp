using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FluentAssertions.Extensions;
using ToDo.API.Contracts.Responses;

namespace IntegrationTests.EndpointTests;

[Collection(ToDoApiCollection.CollectionName)]
public class GetToDoItemEndpointTests(ToDoApiFactory toDoApiFactory) : IAsyncLifetime
{
    private readonly HttpClient _client = toDoApiFactory.HttpClient;
    private readonly Func<Task> _resetDatabase = toDoApiFactory.ResetDatabase;

    [Fact]
    public async Task ToDoItemExists_ShouldReturnToDoItem()
    {
        var addToDoItem = await _client.AddToDoItem();

        var response = await _client.GetAsync(TestConstants.ToDoApiUri + $"/{addToDoItem.Id}");
        var toDoItemResponse = await response.Content.ReadFromJsonAsync<ToDoItemResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        toDoItemResponse.Should().BeEquivalentTo(addToDoItem,
            opt => opt.Using<DateTimeOffset>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 10.Milliseconds()))
                .WhenTypeIs<DateTimeOffset>());
    }

    [Fact]
    public async Task ToDoItemDoesNotExist_ShouldReturnNotFound()
    {
        const int notExistingId = 1;

        var response = await _client.GetAsync(TestConstants.ToDoApiUri + $"/{notExistingId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}