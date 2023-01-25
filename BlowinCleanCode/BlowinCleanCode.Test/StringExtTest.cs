using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Model;
using FluentAssertions;
using Xunit;

namespace BlowinCleanCode.Test;

public class StringExtTest
{
    [Theory]
    [InlineData("", "1")]
    [InlineData("a", "1")]
    [InlineData("a1", "1")]
    [InlineData("a1a", "1")]
    [InlineData("a1 1", "1")]
    [InlineData("a11", "1")]
    public void SplitEnumerator(string inputData, string separator)
    {
        var result = inputData.SplitEnumerator(separator).Select(e => e.ToString()).ToArray();
        var expectResult = inputData.Split(separator);

        result.Should().Equal(expectResult);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData(" 1")]
    [InlineData(" 1 ")]
    [InlineData("  1 ")]
    [InlineData("1  1 ")]
    [InlineData("1  1 1")]
    [InlineData("1  1 1 ")]
    [InlineData("1  1 1      ")]
    [InlineData("1 1 1 1")]
    [InlineData("1 1 1 1 ")]
    [InlineData("1 1 1 1     ")]
    [InlineData("       1 1 1 1     ")]
    [InlineData("       1 1    1 1     ")]
    public void TrimStart(string inputData)
    {
        var slice = new StringSlice(inputData).TrimStart();

        var expectResult = inputData.TrimStart();

        slice.ToString().Should().Be(expectResult);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData(" 1")]
    [InlineData(" 1 ")]
    [InlineData("  1 ")]
    [InlineData("1  1 ")]
    [InlineData("1  1 1")]
    [InlineData("1  1 1 ")]
    [InlineData("1  1 1      ")]
    [InlineData("1 1 1 1")]
    [InlineData("1 1 1 1 ")]
    [InlineData("1 1 1 1     ")]
    [InlineData("       1 1 1 1     ")]
    [InlineData("       1 1    1 1     ")]
    public void TrimEnd(string inputData)
    {
        var slice = new StringSlice(inputData).TrimEnd();

        var expectResult = inputData.TrimEnd();

        slice.ToString().Should().Be(expectResult);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData(" 1")]
    [InlineData(" 1 ")]
    [InlineData("  1 ")]
    [InlineData("1  1 ")]
    [InlineData("1  1 1")]
    [InlineData("1  1 1 ")]
    [InlineData("1  1 1      ")]
    [InlineData("1 1 1 1")]
    [InlineData("1 1 1 1 ")]
    [InlineData("1 1 1 1     ")]
    [InlineData("       1 1 1 1     ")]
    [InlineData("       1 1    1 1     ")]
    public void Trim(string inputData)
    {
        var slice = new StringSlice(inputData).Trim();

        var expectResult = inputData.Trim();

        slice.ToString().Should().Be(expectResult);
    }

    [Theory]
    [InlineData("", 0, "", 0, 0)]
    [InlineData("T", 0, "T", 0, 1)]
    [InlineData(" T", 1, "  T", 2, 1)]
    [InlineData(" T ", 1, "  T  ", 2, 1)]
    [InlineData("Hello", 0, " Hello ", 1, 4)]
    public void StringSliceEquals(string left, int startLeftPosition, string right, int startRightPosition, int lengthSlice)
    {
        var leftSlice = left.AsStringSlice(startLeftPosition, lengthSlice);
        var rightSlice = right.AsStringSlice(startRightPosition, lengthSlice);

        leftSlice.Should().Be(rightSlice);
    }
}
