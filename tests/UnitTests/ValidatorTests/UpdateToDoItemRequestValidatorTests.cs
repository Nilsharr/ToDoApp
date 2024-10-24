using FluentValidation.TestHelper;
using NSubstitute;
using ToDo.API.Contracts.Requests;
using ToDo.API.Validators;
using ToDo.Domain.Interfaces;

namespace UnitTests.ValidatorTests;

public class UpdateToDoItemRequestValidatorTests
{
    private readonly IToDoRepository _toDoRepository = Substitute.For<IToDoRepository>();

    private readonly UpdateToDoItemRequestValidator _updateToDoItemRequestValidator;

    public UpdateToDoItemRequestValidatorTests()
    {
        _updateToDoItemRequestValidator = new UpdateToDoItemRequestValidator(_toDoRepository);
    }

    [Theory]
    [MemberData(nameof(GetValidRequestValues))]
    public async Task AllValidFields_ShouldNotHaveAnyValidationErrors(string title, int completionPercentage,
        string? description,
        DateTimeOffset? expiry)
    {
        var updateToDoItemRequest = new UpdateToDoItemRequest(1, title, completionPercentage, description, expiry);
        var now = DateTimeOffset.UtcNow;
        _toDoRepository.GetExpiryDate(Arg.Any<int>()).Returns(now);

        var result = await _updateToDoItemRequestValidator.TestValidateAsync(updateToDoItemRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("Xyz")]
    [InlineData("Check mail")]
    [InlineData("A lorem ipsum dolor sit amet")]
    public void ValidTitle_ShouldNotHaveTitleValidationErrors(string title)
    {
        var updateToDoItemRequest = new UpdateToDoItemRequest(1, title, 50);

        var result = _updateToDoItemRequestValidator.TestValidate(updateToDoItemRequest);

        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Lorem ipsum")]
    public void ValidDescription_ShouldNotHaveDescriptionValidationErrors(string? description)
    {
        var updateToDoItemRequest = new UpdateToDoItemRequest(1, "Title", 25, description);

        var result = _updateToDoItemRequestValidator.TestValidate(updateToDoItemRequest);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(99)]
    [InlineData(100)]
    public void ValidCompletionPercentage_ShouldNotHaveCompletionPercentageValidationErrors(int percentage)
    {
        var updateToDoItemRequest = new UpdateToDoItemRequest(1, "Title", percentage);

        var result = _updateToDoItemRequestValidator.TestValidate(updateToDoItemRequest);

        result.ShouldNotHaveValidationErrorFor(x => x.CompletionPercentage);
    }

    [Theory]
    [MemberData(nameof(GetValidExpiryDates))]
    public async Task ValidExpiry_ShouldNotHaveExpiryValidationErrors(DateTimeOffset? expiry)
    {
        var updateToDoItemRequest = new UpdateToDoItemRequest(1, "Title", 25, null, expiry);
        var now = DateTimeOffset.UtcNow;
        _toDoRepository.GetExpiryDate(Arg.Any<int>()).Returns(now);

        var result = await _updateToDoItemRequestValidator.TestValidateAsync(updateToDoItemRequest);

        result.ShouldNotHaveValidationErrorFor(x => x.Expiry);
    }

    [Theory]
    [MemberData(nameof(GetPastExpiryDates))]
    public async Task ExpiryDidNotGetUpdated_ShouldNotHaveExpiryValidationErrors(DateTimeOffset? expiry)
    {
        var updateToDoItemRequest = new UpdateToDoItemRequest(1, "Title", 60, null, expiry);
        _toDoRepository.GetExpiryDate(Arg.Any<int>()).Returns(expiry);

        var result = await _updateToDoItemRequestValidator.TestValidateAsync(updateToDoItemRequest);

        result.ShouldNotHaveValidationErrorFor(x => x.Expiry);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void EmptyTitle_ShouldHaveValidationError(string title)
    {
        var updateToDoItemRequest = new UpdateToDoItemRequest(1, title, 33);

        var result = _updateToDoItemRequestValidator.TestValidate(updateToDoItemRequest);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TitleLengthTooSmall_ShouldHaveValidationError(int length)
    {
        var updateToDoItemRequest = new UpdateToDoItemRequest(1, new string('a', length), 75);

        var result = _updateToDoItemRequestValidator.TestValidate(updateToDoItemRequest);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(129)]
    [InlineData(150)]
    [InlineData(200)]
    public void TitleLengthTooBig_ShouldHaveValidationError(int length)
    {
        var updateToDoItemRequest = new UpdateToDoItemRequest(1, new string('a', length), 80);

        var result = _updateToDoItemRequestValidator.TestValidate(updateToDoItemRequest);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(513)]
    [InlineData(600)]
    [InlineData(1024)]
    public void DescriptionLengthTooBig_ShouldHaveValidationError(int length)
    {
        var updateToDoItemRequest = new UpdateToDoItemRequest(1, "Title", 40, new string('a', length));

        var result = _updateToDoItemRequestValidator.TestValidate(updateToDoItemRequest);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(101)]
    [InlineData(200)]
    public void PercentageIsInvalid_ShouldHaveValidationError(int percentage)
    {
        var updateToDoItemRequest = new UpdateToDoItemRequest(1, "Title", percentage);

        var result = _updateToDoItemRequestValidator.TestValidate(updateToDoItemRequest);

        result.ShouldHaveValidationErrorFor(x => x.CompletionPercentage);
    }

    [Theory]
    [MemberData(nameof(GetPastExpiryDates))]
    public async Task ExpiryIsInPast_ShouldHaveValidationError(DateTimeOffset? expiry)
    {
        var updateToDoItemRequest = new UpdateToDoItemRequest(1, "Title", 10, null, expiry);
        var now = DateTimeOffset.UtcNow;
        _toDoRepository.GetExpiryDate(Arg.Any<int>()).Returns(now);

        var result = await _updateToDoItemRequestValidator.TestValidateAsync(updateToDoItemRequest);

        result.ShouldHaveValidationErrorFor(x => x.Expiry);
    }

    public static TheoryData<string, int, string?, DateTimeOffset?> GetValidRequestValues() => new()
    {
        { "Title", 0, null, null },
        { "Title", 50, "Description", null },
        { "Title", 100, null, DateTimeOffset.UtcNow.AddDays(2) },
        { "Xyz", 25, "Lorem ipsum", DateTimeOffset.UtcNow.AddDays(3) }
    };

    public static TheoryData<DateTimeOffset?> GetValidExpiryDates()
    {
        var now = DateTimeOffset.Now;
        return
        [
            null,
            now.AddMinutes(1),
            now.AddHours(2),
            now.AddDays(4),
            now.AddMonths(8)
        ];
    }

    public static TheoryData<DateTimeOffset?> GetPastExpiryDates()
    {
        var now = DateTimeOffset.Now;
        return
        [
            DateTimeOffset.MinValue,
            now.AddMinutes(-1),
            now.AddHours(-2),
            now.AddDays(-4),
            now.AddMonths(-8)
        ];
    }
}