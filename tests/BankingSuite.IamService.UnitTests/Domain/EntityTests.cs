using BankingSuite.BuildingBlocks.Domain.Abstractions;
using FluentAssertions;

namespace BankingSuite.IamService.UnitTests.Domain;

public class EntityTests
{
    private sealed class DummyEntity(Guid id) : Entity<Guid>(id);

    [Fact]
    public void Equals_Should_Return_True_For_Same_Id()
    {
        var id = Guid.NewGuid();
        var a = new DummyEntity(id);
        var b = new DummyEntity(id);

        a.Equals(b).Should().BeTrue();
        (a == b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Equals_Should_Return_False_For_Default_Ids()
    {
        var a = new DummyEntity(Guid.Empty);
        var b = new DummyEntity(Guid.NewGuid());

        a.Equals(b).Should().BeFalse();
        (a != b).Should().BeTrue();
    }
}
