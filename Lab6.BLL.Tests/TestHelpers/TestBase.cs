using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NUnit.Framework;
using System.Linq;

namespace ContentLibrary.Tests.TestHelpers
{
    public abstract class TestBase
    {
        public IFixture Fixture { get; private set; }

        [SetUp]
        public virtual void Setup()
        {
            // Створюємо Fixture з NSubstitute
            Fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

            // Виправлення циклічних посилань
            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [TearDown]
        public virtual void Cleanup()
        {
            // Метод для звільнення ресурсів
        }
    }
}