using BuildingBlocks;
using Xunit;

namespace BuildingBlocks.UnitTests;

public class ResultTests
{
    [Fact]
    public void Success_Should_Set_IsSuccess_True_And_IsFailure_False()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_Should_Set_IsSuccess_False_And_Error_Message()
    {
        // Arrange
        var errorMessage = "Something went wrong";

        // Act
        var result = Result.Failure(errorMessage);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(errorMessage, result.Error);
    }

    [Fact]
    public void ResultOfT_Success_Should_Contain_Value()
    {
        // Arrange
        var value = 42;

        // Act
        var result = Result<int>.Success(value);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void ResultOfT_Failure_Should_Not_Contain_Value()
    {
        // Arrange
        var errorMessage = "Failure with generic type";

        // Act
        var result = Result<int>.Failure(errorMessage);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(errorMessage, result.Error);
        Assert.Equal(default, result.Value);
    }
}