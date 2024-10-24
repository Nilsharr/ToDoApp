using Ardalis.Result;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Domain.Services;

namespace UnitTests.ServiceTests;

public class ToDoServiceTests
{
    private readonly IToDoRepository _toDoRepository = Substitute.For<IToDoRepository>();
    private readonly ToDoService _toDoService;

    public ToDoServiceTests()
    {
        var logger = Substitute.For<ILogger<ToDoService>>();
        _toDoService = new ToDoService(_toDoRepository, logger);
    }

    [Fact]
    public async Task Update_ToDoItemExists_ShouldReturnUpdatedItem()
    {
        const int id = 1;
        var existingToDoItem = new ToDoItem { Id = id, Title = "Old title" };
        var updatedToDoItem = new ToDoItem { Id = id, Title = "New title" };
        _toDoRepository.GetById(id).Returns(existingToDoItem);

        var result = await _toDoService.Update(updatedToDoItem);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_ToDoItemExists_ShouldUpdateTitle()
    {
        const int id = 1;
        const string updatedTitle = "Updated title";
        var existingToDoItem = new ToDoItem { Id = id, Title = "Old title" };
        var updatedToDoItem = new ToDoItem { Id = id, Title = updatedTitle };
        _toDoRepository.GetById(id).Returns(existingToDoItem);

        var result = await _toDoService.Update(updatedToDoItem);

        result.Value.Title.Should().Be(updatedTitle);
    }

    [Fact]
    public async Task Update_ToDoItemExists_ShouldUpdateDescription()
    {
        const int id = 1;
        const string updatedDescription = "Updated description";
        var existingToDoItem = new ToDoItem { Id = id, Description = "Old description" };
        var updatedToDoItem = new ToDoItem { Id = id, Description = updatedDescription };
        _toDoRepository.GetById(id).Returns(existingToDoItem);

        var result = await _toDoService.Update(updatedToDoItem);

        result.Value.Description.Should().Be(updatedDescription);
    }

    [Fact]
    public async Task Update_ToDoItemExists_ShouldUpdateCompletionPercentage()
    {
        const int id = 1;
        const int updatedCompletionPercentage = 50;
        var existingToDoItem = new ToDoItem { Id = id, CompletionPercentage = 0 };
        var updatedToDoItem = new ToDoItem { Id = id, CompletionPercentage = updatedCompletionPercentage };
        _toDoRepository.GetById(id).Returns(existingToDoItem);

        var result = await _toDoService.Update(updatedToDoItem);

        result.Value.CompletionPercentage.Should().Be(updatedCompletionPercentage);
    }

    [Fact]
    public async Task Update_ToDoItemExists_ShouldUpdateExpiry()
    {
        const int id = 1;
        var updatedExpiry = DateTimeOffset.UtcNow.AddDays(5);
        var existingToDoItem = new ToDoItem { Id = id, Expiry = DateTimeOffset.UtcNow.AddDays(2) };
        var updatedToDoItem = new ToDoItem { Id = id, Expiry = updatedExpiry };
        _toDoRepository.GetById(id).Returns(existingToDoItem);

        var result = await _toDoService.Update(updatedToDoItem);

        result.Value.Expiry.Should().Be(updatedExpiry);
    }

    [Fact]
    public async Task Update_ToDoItemExists_ShouldNotUpdateCreatedOn()
    {
        const int id = 1;
        var now = DateTimeOffset.UtcNow;
        var existingToDoItem = new ToDoItem { Id = id, CreatedOn = now };
        var updatedToDoItem = new ToDoItem { Id = id, CreatedOn = DateTimeOffset.MinValue };
        _toDoRepository.GetById(id).Returns(existingToDoItem);

        var result = await _toDoService.Update(updatedToDoItem);

        result.Value.CreatedOn.Should().Be(now);
    }


    [Fact]
    public async Task Update_ToDoItemDoesNotExist_ShouldReturnNotFound()
    {
        const int id = 99;
        var nonExistentToDoItem = new ToDoItem { Id = id, Title = "Non-existent" };
        _toDoRepository.GetById(id).ReturnsNull();

        var result = await _toDoService.Update(nonExistentToDoItem);

        result.Status.Should().Be(ResultStatus.NotFound);
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task SetCompletionPercentage_ToDoItemExists_ShouldReturnSuccessResult()
    {
        const int id = 1;
        const int newPercentage = 20;
        var existingToDoItem = new ToDoItem { Id = id, CompletionPercentage = 10 };
        _toDoRepository.GetById(id).Returns(existingToDoItem);

        var result = await _toDoService.SetCompletionPercentage(id, newPercentage);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(0, 50)]
    [InlineData(0, 99)]
    [InlineData(0, 100)]
    [InlineData(1, 0)]
    [InlineData(1, 100)]
    [InlineData(50, 0)]
    [InlineData(50, 100)]
    [InlineData(100, 0)]
    [InlineData(100, 1)]
    [InlineData(100, 50)]
    [InlineData(100, 99)]
    public async Task SetCompletionPercentage_ToDoItemExists_ShouldUpdatePercentage(int currentPercentage,
        int expectedPercentage)
    {
        const int id = 1;
        var existingToDoItem = new ToDoItem { Id = id, CompletionPercentage = currentPercentage };
        _toDoRepository.GetById(id).Returns(existingToDoItem);

        var result = await _toDoService.SetCompletionPercentage(id, expectedPercentage);

        result.Value.CompletionPercentage.Should().Be(expectedPercentage);
    }

    [Fact]
    public async Task SetCompletionPercentage_ToDoItemDoesNotExist_ShouldReturnNotFound()
    {
        const int id = 50;
        const int newPercentage = 75;
        _toDoRepository.GetById(id).ReturnsNull();

        var result = await _toDoService.SetCompletionPercentage(id, newPercentage);

        result.Status.Should().Be(ResultStatus.NotFound);
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task MarkAsDone_ToDoItemExists_ShouldReturnSuccessResult()
    {
        const int id = 1;
        var existingToDoItem = new ToDoItem { Id = id, CompletionPercentage = 33 };
        _toDoRepository.GetById(id).Returns(existingToDoItem);

        var result = await _toDoService.MarkAsDone(id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(33)]
    [InlineData(50)]
    [InlineData(66)]
    [InlineData(75)]
    [InlineData(99)]
    [InlineData(100)]
    public async Task MarkAsDone_ToDoItemExists_ShouldSetAsDone(int currentPercentage)
    {
        const int id = 1;
        const int expectedPercentage = 100;
        var existingToDoItem = new ToDoItem { Id = id, CompletionPercentage = currentPercentage };
        _toDoRepository.GetById(id).Returns(existingToDoItem);

        var result = await _toDoService.MarkAsDone(id);

        result.Value.CompletionPercentage.Should().Be(expectedPercentage);
    }

    [Fact]
    public async Task MarkAsDone_ToDoItemDoesNotExist_ShouldReturnNotFound()
    {
        const int id = 70;
        _toDoRepository.GetById(id).ReturnsNull();

        var result = await _toDoService.MarkAsDone(id);

        result.Status.Should().Be(ResultStatus.NotFound);
        result.Value.Should().BeNull();
    }
}