using BankingSuite.BuildingBlocks.Domain.Abstractions;
using FluentAssertions;
using Xunit;

namespace BankingSuite.IamService.UnitTests.Common;

public class ResultTests
{
    [Fact]
    public void Success_Should_Set_IsSuccess_True_And_Error_Null()
    {
        // Arrange & Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_Should_Set_IsSuccess_False_And_Error_Message()
    {
        // Arrange
        const string errorMessage = "Something went wrong";

        // Act
        var result = Result.Failure(errorMessage);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public void Generic_Success_Should_Expose_Value()
    {
        // Arrange
        const string expected = "OK";

        // Act
        var result = Result.Success(expected);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expected);
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Generic_Failure_Should_Set_Error_And_Default_Value()
    {
        const string errorMessage = "fail";

        var result = Result.Failure<int>(errorMessage);

        result.IsSuccess.Should().BeFalse();
        result.Value.Should().Be(default);
        result.Error.Should().Be(errorMessage);
    }
}
