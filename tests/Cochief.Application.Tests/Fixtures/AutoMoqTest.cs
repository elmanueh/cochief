namespace Cochief.Application.Tests.Fixtures;

using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;

public abstract class AutoMoqTest
{
    private IFixture Fixture { get; } = new Fixture()
        .Customize(new AutoMoqCustomization());

    protected Mock<T> Freeze<T>() where T : class => Fixture.Freeze<Mock<T>>();

    protected void Inject<T>(T instance) => Fixture.Inject(instance);

    protected T Create<T>() => Fixture.Create<T>();
}
