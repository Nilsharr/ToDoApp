using FluentValidation.TestHelper;
using ToDo.API.Contracts.Requests;
using ToDo.API.Validators;

namespace UnitTests.ValidatorTests;

public class GetIncomingToDoItemsRequestValidatorTests
{
    private readonly GetIncomingToDoItemsRequestValidator _getIncomingToDoItemsRequestValidator = new();

    [Theory]
    [InlineData("2024-10-15", "2024-10-15")]
    [InlineData("2024-10-22", "2024-10-23")]
    [InlineData("2024-10-22", "2024-10-29")]
    [InlineData("2024-10-27", "2024-11-20")]
    [InlineData("2023-06-25", "2024-06-25")]
    public void EndDateIsGreaterThanStartDate_ShouldNotHaveValidationError(string startDateString,
        string? endDateString)
    {
        DateOnly? endDate = endDateString is null ? null : DateOnly.Parse(endDateString);
        var getIncomingToDoItemsRequest = new GetIncomingToDoItemsRequest(DateOnly.Parse(startDateString), endDate);

        var result = _getIncomingToDoItemsRequestValidator.TestValidate(getIncomingToDoItemsRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("2024-10-20")]
    [InlineData("2024-10-30")]
    [InlineData("2024-12-27")]
    [InlineData("2025-02-01")]
    public void OnlyStartDateGiven_ShouldNotHaveValidationError(string startDateString)
    {
        var getIncomingToDoItemsRequest = new GetIncomingToDoItemsRequest(DateOnly.Parse(startDateString), null);

        var result = _getIncomingToDoItemsRequestValidator.TestValidate(getIncomingToDoItemsRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("2024-10-21", "2024-10-20")]
    [InlineData("2024-09-14", "2024-08-18")]
    [InlineData("2024-05-05", "2023-06-12")]
    public void EndDateIsLessThanStartDate_ShouldHaveValidationError(string startDateString, string? endDateString)
    {
        DateOnly? endDate = endDateString is null ? null : DateOnly.Parse(endDateString);
        var getIncomingToDoItemsRequest = new GetIncomingToDoItemsRequest(DateOnly.Parse(startDateString), endDate);

        var result = _getIncomingToDoItemsRequestValidator.TestValidate(getIncomingToDoItemsRequest);

        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }
}