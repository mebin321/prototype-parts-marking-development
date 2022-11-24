namespace WebApi.Test.Unit.Common
{
    using System;
    using Shouldly;
    using WebApi.Common.ResourceVersioning;
    using WebApi.Data;
    using Xunit;

    public class ETagGeneratorTestSuite
    {
        [Fact]
        public void ETagFrom_ShouldRejectNullInput()
        {
            var generator = new ETagGenerator();

            Should.Throw<ArgumentNullException>(() => generator.ETagFrom(null));
        }

        [Fact]
        public void ETagFrom_ShouldGenerateDifferentResultsForDifferentModifiedAtValues()
        {
            var generator = new ETagGenerator();

            var dateTime = DateTimeOffset.UtcNow;
            var entity1 = new AuditableEntityStub { ModifiedAt = dateTime };
            var entity2 = new AuditableEntityStub { ModifiedAt = dateTime.AddMilliseconds(1) };

            var result1 = generator.ETagFrom(entity1);
            var result2 = generator.ETagFrom(entity2);

            result1.ShouldNotBe(result2);
        }

        [Fact]
        public void ETagFrom_ResultingValueShouldBeDeterministic()
        {
            var generator = new ETagGenerator();
            var entity = new AuditableEntityStub { ModifiedAt = DateTimeOffset.UtcNow };

            var result1 = generator.ETagFrom(entity);
            var result2 = generator.ETagFrom(entity);

            result1.ShouldBe(result2);
        }

        [Fact]
        public void ETagFrom_ShouldReturnEmptyStringIfModifiedAtValueIsNotSet()
        {
            var generator = new ETagGenerator();
            var entity = new AuditableEntityStub { ModifiedAt = default };

            var result = generator.ETagFrom(entity);

            result.ShouldBe(string.Empty);
        }

        private class AuditableEntityStub : IAuditableEntity
        {
            public DateTimeOffset? CreatedAt { get; set; }
            public DateTimeOffset? ModifiedAt { get; set; }
            public DateTimeOffset? DeletedAt { get; set; }
        }
    }
}
