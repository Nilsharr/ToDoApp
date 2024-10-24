namespace IntegrationTests;

[CollectionDefinition(CollectionName)]
public class ToDoApiCollection : ICollectionFixture<ToDoApiFactory>
{
    public const string CollectionName = nameof(ToDoApiCollection);
}