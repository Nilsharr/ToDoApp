using FluentValidation.TestHelper;
using ToDo.API.Contracts.Requests;
using ToDo.API.Validators;

namespace UnitTests.ValidatorTests;

public class AddToDoItemRequestValidatorTests
{
    private readonly AddToDoItemRequestValidator _addToDoItemRequestValidator = new();

    [Theory]
    [MemberData(nameof(GetValidRequestValues))]
    public void AllValidFields_ShouldNotHaveAnyValidationErrors(string title, string? description,
        DateTimeOffset? expiry)
    {
        var addToDoItemRequest = new AddToDoItemRequest(title, description, expiry);

        var result = _addToDoItemRequestValidator.TestValidate(addToDoItemRequest);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("Xyz")]
    [InlineData("Go to the gym")]
    [InlineData("A lorem ipsum dolor sit amet")]
    public void ValidTitle_ShouldNotHaveTitleValidationErrors(string title)
    {
        var addToDoItemRequest = new AddToDoItemRequest(title);

        var result = _addToDoItemRequestValidator.TestValidate(addToDoItemRequest);

        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Lorem ipsum")]
    public void ValidDescription_ShouldNotHaveDescriptionValidationErrors(string? description)
    {
        var addToDoItemRequest = new AddToDoItemRequest("Title", description);

        var result = _addToDoItemRequestValidator.TestValidate(addToDoItemRequest);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [MemberData(nameof(GetValidExpiryDates))]
    public void ValidExpiry_ShouldNotHaveExpiryValidationErrors(DateTimeOffset? expiry)
    {
        var addToDoItemRequest = new AddToDoItemRequest("Title", null, expiry);

        var result = _addToDoItemRequestValidator.TestValidate(addToDoItemRequest);

        result.ShouldNotHaveValidationErrorFor(x => x.Expiry);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void EmptyTitle_ShouldHaveValidationError(string title)
    {
        var addToDoItemRequest = new AddToDoItemRequest(title);

        var result = _addToDoItemRequestValidator.TestValidate(addToDoItemRequest);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TitleLengthTooSmall_ShouldHaveValidationError(int length)
    {
        var addToDoItemRequest = new AddToDoItemRequest(new string('a', length));

        var result = _addToDoItemRequestValidator.TestValidate(addToDoItemRequest);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(129)]
    [InlineData(150)]
    [InlineData(200)]
    public void TitleLengthTooBig_ShouldHaveValidationError(int length)
    {
        var addToDoItemRequest = new AddToDoItemRequest(new string('a', length));

        var result = _addToDoItemRequestValidator.TestValidate(addToDoItemRequest);

        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [InlineData(513)]
    [InlineData(600)]
    [InlineData(1024)]
    public void DescriptionLengthTooBig_ShouldHaveValidationError(int length)
    {
        var addToDoItemRequest = new AddToDoItemRequest("Title", new string('a', length));

        var result = _addToDoItemRequestValidator.TestValidate(addToDoItemRequest);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [MemberData(nameof(GetPastExpiryDates))]
    public void ExpiryIsInPast_ShouldHaveValidationError(DateTimeOffset? expiry)
    {
        var addToDoItemRequest = new AddToDoItemRequest("Title", null, expiry);

        var result = _addToDoItemRequestValidator.TestValidate(addToDoItemRequest);

        result.ShouldHaveValidationErrorFor(x => x.Expiry);
    }

    public static TheoryData<string, string?, DateTimeOffset?> GetValidRequestValues() => new()
    {
        { "Title", null, null },
        { "Title", "Description", null },
        { "Title", null, DateTimeOffset.UtcNow.AddDays(2) },
        { "Xyz", "Lorem ipsum", DateTimeOffset.UtcNow.AddDays(3) }
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