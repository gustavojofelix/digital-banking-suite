using Xunit;

namespace BuildingBlocks.UnitTests;

public class ValueObjectTests
{
    [Fact]
    public void Two_ValueObjects_With_Same_Values_Should_Be_Equal()
    {
        // Arrange
        var a = new TestMoney(100m, "USD");
        var b = new TestMoney(100m, "USD");

        // Act & Assert
        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.False(a != b);
    }

    [Fact]
    public void Two_ValueObjects_With_Different_Values_Should_Not_Be_Equal()
    {
        // Arrange
        var a = new TestMoney(100m, "USD");
        var b = new TestMoney(200m, "USD");

        // Act & Assert
        Assert.NotEqual(a, b);
        Assert.True(a != b);
        Assert.False(a == b);
    }

    [Fact]
    public void ValueObject_Should_Have_Consistent_HashCode_For_Equal_Values()
    {
        // Arrange
        var a = new TestMoney(50m, "EUR");
        var b = new TestMoney(50m, "EUR");

        // Act
        var hashA = a.GetHashCode();
        var hashB = b.GetHashCode();

        // Assert
        Assert.Equal(hashA, hashB);
    }
}