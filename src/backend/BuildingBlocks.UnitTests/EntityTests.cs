using Xunit;

namespace BuildingBlocks.UnitTests;

public class EntityTests
{
    [Fact]
    public void Entities_With_Same_Id_Should_Be_Equal()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entityA = new TestEntity(id, "A");
        var entityB = new TestEntity(id, "B");

        // Act & Assert
        Assert.Equal(entityA, entityB);
        Assert.True(entityA == entityB);
        Assert.False(entityA != entityB);
    }

    [Fact]
    public void Entities_With_Different_Id_Should_Not_Be_Equal()
    {
        // Arrange
        var entityA = new TestEntity(Guid.NewGuid(), "A");
        var entityB = new TestEntity(Guid.NewGuid(), "A");

        // Act & Assert
        Assert.NotEqual(entityA, entityB);
        Assert.True(entityA != entityB);
        Assert.False(entityA == entityB);
    }
}