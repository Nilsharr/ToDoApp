using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ToDo.API.Contracts.Responses;

namespace IntegrationTests.EndpointTests;

[Collection(ToDoApiCollection.CollectionName)]
public class GetIncomingToDoItemsEndpointTests(ToDoApiFactory toDoApiFactory) : IAsyncLifetime
{
    private readonly HttpClient _client = toDoApiFactory.HttpClient;
    private readonly Func<Task> _resetDatabase = toDoApiFactory.ResetDatabase;

    [Fact]
    public async Task OnlyStartDateSet_ShouldReturnToDoItemsFromThatDay()
    {
        const int expectedCount = 3;
        var tomorrow = DateTimeOffset.UtcNow.AddDays(1);
        var tomorrowDateOnly = tomorrow.ToString("yyyy-MM-dd");
        var threeDaysLater = DateTimeOffset.UtcNow.AddDays(3);
        await AddToDoItems(expectedCount, tomorrow);
        await AddToDoItems(2, threeDaysLater);

        var response = await _client.GetAsync(TestConstants.ToDoApiUri + $"/incoming?startDate={tomorrowDateOnly}");
        var toDoItemResponse = await response.Content.ReadFromJsonAsync<List<ToDoItemResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        toDoItemResponse.Should().NotBeNull();
        toDoItemResponse.Should().HaveCount(expectedCount);
    }

    [Fact]
    public async Task DateRangeSet_ShouldReturnToDoItemsFromThatPeriod()
    {
        const int expectedCount = 6;
        var tomorrow = DateTimeOffset.UtcNow.AddDays(1);
        var twoDaysLater = DateTimeOffset.UtcNow.AddDays(2);
        var threeDaysLater = DateTimeOffset.UtcNow.AddDays(3);
        var sixDaysLater = DateTimeOffset.UtcNow.AddDays(6);
        var startDate = tomorrow.ToString("yyyy-MM-dd");
        var endDate = threeDaysLater.ToString("yyyy-MM-dd");
        await AddToDoItems(3, tomorrow);
        await AddToDoItems(1, twoDaysLater);
        await AddToDoItems(2, threeDaysLater);
        await AddToDoItems(4, sixDaysLater);

        var response =
            await _client.GetAsync(TestConstants.ToDoApiUri + $"/incoming?startDate={startDate}&endDate={endDate}");
        var toDoItemResponse = await response.Content.ReadFromJsonAsync<List<ToDoItemResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        toDoItemResponse.Should().NotBeNull();
        toDoItemResponse.Should().HaveCount(expectedCount);
    }

    [Fact]
    public async Task NoIncomingToDoItemsOnDay_ShouldReturnEmptyList()
    {
        var tomorrowDateOnly = DateTimeOffset.UtcNow.AddDays(1).ToString("yyyy-MM-dd");
        var threeDaysLater = DateTimeOffset.UtcNow.AddDays(3);
        await _client.AddToDoItem(threeDaysLater);
        await _client.AddToDoItem(threeDaysLater);

        var response = await _client.GetAsync(TestConstants.ToDoApiUri + $"/incoming?startDate={tomorrowDateOnly}");
        var toDoItemResponse = await response.Content.ReadFromJsonAsync<List<ToDoItemResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        toDoItemResponse.Should().BeEmpty();
    }

    [Fact]
    public async Task NoIncomingToDoItemsInRange_ShouldReturnEmptyList()
    {
        var tomorrowDateOnly = DateTimeOffset.UtcNow.AddDays(1).ToString("yyyy-MM-dd");
        var weekLaterDateOnly = DateTimeOffset.UtcNow.AddDays(7).ToString("yyyy-MM-dd");
        var monthLater = DateTimeOffset.UtcNow.AddMonths(1);
        await _client.AddToDoItem(monthLater);
        await _client.AddToDoItem(monthLater);

        var response = await _client.GetAsync(TestConstants.ToDoApiUri +
                                              $"/incoming?startDate={tomorrowDateOnly}&endDate={weekLaterDateOnly}");
        var toDoItemResponse = await response.Content.ReadFromJsonAsync<List<ToDoItemResponse>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        toDoItemResponse.Should().BeEmpty();
    }

    [Fact]
    public async Task StartDateNotSet_ShouldReturnBadRequest()
    {
        var endDate = DateTimeOffset.UtcNow.AddDays(2).ToString("yyyy-MM-dd");

        var response = await _client.GetAsync(TestConstants.ToDoApiUri + $"/incoming?endDate={endDate}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task StartDateGreaterThanEndDate_ShouldReturnBadRequest()
    {
        var startDate = DateTimeOffset.UtcNow.AddDays(4).ToString("yyyy-MM-dd");
        var endDate = DateTimeOffset.UtcNow.AddDays(2).ToString("yyyy-MM-dd");

        var response =
            await _client.GetAsync(TestConstants.ToDoApiUri + $"/incoming?startDate={startDate}&endDate={endDate}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    private async Task AddToDoItems(int count, DateTimeOffset expiryDate)
    {
        for (var i = 0; i < count; i++)
        {
            await _client.AddToDoItem(expiryDate);
        }
    }
}