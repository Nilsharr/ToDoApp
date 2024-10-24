using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ToDo.API.Contracts.Responses;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Domain.Models;

namespace IntegrationTests.EndpointTests;

[Collection(ToDoApiCollection.CollectionName)]
public class GetAllToDoItemsEndpointTests(ToDoApiFactory toDoApiFactory) : IAsyncLifetime
{
    private readonly HttpClient _client = toDoApiFactory.HttpClient;

    private readonly IToDoRepository _toDoRepository =
        toDoApiFactory.Services.CreateScope().ServiceProvider.GetRequiredService<IToDoRepository>();

    private readonly Func<Task> _resetDatabase = toDoApiFactory.ResetDatabase;

    [Fact]
    public async Task DatabaseContainsToDoItems_ShouldReturnPaginatedListWithToDoItems()
    {
        const int pageIndex = 0;
        const int pageSize = 20;
        const int expectedCount = 5;
        await AddToDoItems(expectedCount, false);

        var response = await _client.GetAsync(TestConstants.ToDoApiUri + $"?pageIndex={pageIndex}&pageSize={pageSize}");
        var toDoItemResponse = await response.Content.ReadFromJsonAsync<PaginatedList<ToDoItemResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        toDoItemResponse.Should().NotBeNull();
        toDoItemResponse!.PageIndex.Should().Be(pageIndex);
        toDoItemResponse.PageSize.Should().Be(pageSize);
        toDoItemResponse.TotalCount.Should().Be(expectedCount);
        toDoItemResponse.Data.Should().HaveCount(expectedCount);
    }

    [Fact]
    public async Task EmptyDatabase_ShouldReturnEmptyPaginatedList()
    {
        const int pageIndex = 0;
        const int pageSize = 20;

        var response = await _client.GetAsync(TestConstants.ToDoApiUri + $"?pageIndex={pageIndex}&pageSize={pageSize}");
        var toDoItemResponse = await response.Content.ReadFromJsonAsync<PaginatedList<ToDoItemResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        toDoItemResponse.Should().NotBeNull();
        toDoItemResponse!.PageIndex.Should().Be(pageIndex);
        toDoItemResponse.PageSize.Should().Be(pageSize);
        toDoItemResponse.TotalCount.Should().Be(0);
        toDoItemResponse.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task ShowExpiredFilter_ShouldReturnPaginatedListWithAllToDoItems()
    {
        const int pageIndex = 0;
        const int pageSize = 20;
        const bool showExpired = true;
        const int expectedCount = 4;
        await AddToDoItems(2, false);
        await AddToDoItems(2, true);

        var response = await _client.GetAsync(TestConstants.ToDoApiUri +
                                              $"?pageIndex={pageIndex}&pageSize={pageSize}&expired={showExpired}");
        var toDoItemResponse = await response.Content.ReadFromJsonAsync<PaginatedList<ToDoItemResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        toDoItemResponse!.TotalCount.Should().Be(expectedCount);
        toDoItemResponse.Data.Should().HaveCount(expectedCount);
    }

    [Fact]
    public async Task DoNotShowExpiredFilter_ShouldReturnPaginatedListWithoutExpiredToDoItems()
    {
        const int pageIndex = 0;
        const int pageSize = 20;
        const bool showExpired = false;
        const int expectedCount = 3;
        await AddToDoItems(3, false);
        await AddToDoItems(2, true);

        var response = await _client.GetAsync(TestConstants.ToDoApiUri +
                                              $"?pageIndex={pageIndex}&pageSize={pageSize}&expired={showExpired}");
        var toDoItemResponse = await response.Content.ReadFromJsonAsync<PaginatedList<ToDoItemResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        toDoItemResponse!.TotalCount.Should().Be(expectedCount);
        toDoItemResponse.Data.Should().HaveCount(expectedCount);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    private async Task AddToDoItems(int count, bool addExpired)
    {
        for (var i = 0; i < count; i++)
        {
            if (addExpired)
            {
                await AddExpiredToDoItem();
            }
            else
            {
                await _client.AddToDoItem();
            }
        }
    }

    private async Task AddExpiredToDoItem()
    {
        var expiredDate = DateTimeOffset.UtcNow.AddDays(-1);
        var expired = ToDoItem.Create("Title", null, expiredDate);
        _toDoRepository.Add(expired);
        await _toDoRepository.SaveChangesAsync();
    }
}