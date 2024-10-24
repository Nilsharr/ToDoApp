using Bogus;
using ToDo.API.Contracts.Requests;

namespace IntegrationTests.Fakes;

public sealed class UpdateToDoItemRequestFaker : Faker<UpdateToDoItemRequest>
{
    public UpdateToDoItemRequestFaker(int id, DateTimeOffset? expiryDate = null)
    {
        CustomInstantiator(f =>
            new UpdateToDoItemRequest(id, f.Lorem.Sentence(), f.Random.Int(0, 100), f.Lorem.Sentences(), expiryDate));
    }
}