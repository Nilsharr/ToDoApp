using FluentValidation.TestHelper;
using ToDo.API.Contracts.Requests;
using ToDo.API.Validators;

namespace UnitTests.ValidatorTests;

public class GetAllToDoItemsRequestValidatorTests
{
    private readonly GetAllToDoItemsRequestValidator _getAllToDoItemsRequestValidator = new();

    [Theory]
    [InlineData(0, 1)]
    [InlineData(0, 5)]
    [InlineData(0, 10)]
    [InlineData(0, 100)]
    [InlineData(1, 1)]
    [InlineData(2, 5)]
    [InlineData(5, 10)]
    [InlineData(10, 100)]
    public void BothParametersAreValid_ShouldNotHaveAnyValidationErrors(int pageIndex, int pageSize)
    {
        var getAllToDoItemsRequest = new GetAllToDoItemsRequest(pageIndex, pageSize);

        var result = _getAllToDoItemsRequestValidator.TestValidate(getAllToDoItemsRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(-10)]
    public void PageIndexIsNegative_ShouldHaveValidationError(int pageIndex)
    {
        var getAllToDoItemsRequest = new GetAllToDoItemsRequest(pageIndex);

        var result = _getAllToDoItemsRequestValidator.TestValidate(getAllToDoItemsRequest);

        result.ShouldHaveValidationErrorFor(x => x.PageIndex);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(-10)]
    public void PageSizeIsNotPositiveNumber_ShouldHaveValidationError(int pageSize)
    {
        var getAllToDoItemsRequest = new GetAllToDoItemsRequest(0, pageSize);

        var result = _getAllToDoItemsRequestValidator.TestValidate(getAllToDoItemsRequest);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(150)]
    [InlineData(1000)]
    public void PageSizeTooBig_ShouldHaveValidationError(int pageSize)
    {
        var getAllToDoItemsRequest = new GetAllToDoItemsRequest(0, pageSize);

        var result = _getAllToDoItemsRequestValidator.TestValidate(getAllToDoItemsRequest);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}