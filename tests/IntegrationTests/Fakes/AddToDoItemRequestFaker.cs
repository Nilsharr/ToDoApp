using Bogus;
using ToDo.API.Contracts.Requests;

namespace IntegrationTests.Fakes;

public sealed class AddToDoItemRequestFaker : Faker<AddToDoItemRequest>
{
    public AddToDoItemRequestFaker(DateTimeOffset? expiryDate = null)
    {
        CustomInstantiator(f => new AddToDoItemRequest(f.Lorem.Sentence(), f.Lorem.Sentences(), expiryDate));
    }
}