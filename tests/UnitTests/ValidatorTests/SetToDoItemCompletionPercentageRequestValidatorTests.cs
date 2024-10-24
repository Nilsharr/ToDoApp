using FluentValidation.TestHelper;
using ToDo.API.Contracts.Requests;
using ToDo.API.Validators;

namespace UnitTests.ValidatorTests;

public class SetToDoItemCompletionPercentageRequestValidatorTests
{
    private readonly SetToDoItemCompletionPercentageRequestValidator _setToDoItemCompletionPercentageRequestValidator =
        new();

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(99)]
    [InlineData(100)]
    public void PercentageIsValid_ShouldValidate(int percentage)
    {
        var setToDoItemCompletionPercentageRequest = new SetToDoItemCompletionPercentageRequest(percentage);

        var result =
            _setToDoItemCompletionPercentageRequestValidator.TestValidate(setToDoItemCompletionPercentageRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(101)]
    [InlineData(200)]
    public void PercentageIsInvalid_ShouldHaveValidationError(int percentage)
    {
        var setToDoItemCompletionPercentageRequest = new SetToDoItemCompletionPercentageRequest(percentage);

        var result =
            _setToDoItemCompletionPercentageRequestValidator.TestValidate(setToDoItemCompletionPercentageRequest);

        result.ShouldHaveValidationErrorFor(x => x.Percentage);
    }
}