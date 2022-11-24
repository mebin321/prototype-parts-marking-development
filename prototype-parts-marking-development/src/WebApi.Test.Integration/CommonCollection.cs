namespace WebApi.Test.Integration
{
    using Xunit;

    [Collection(nameof(CommonCollection))]
    public class CommonCollection : ICollectionFixture<TestingFixture>
    {
    }
}