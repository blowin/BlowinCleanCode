using System;
using System.Collections.Generic;
using System.Linq;
using BlowinCleanCode.Extension;
using FluentAssertions;
using Xunit;

namespace BlowinCleanCode.Test;

public class StringExtTest
{
    public static IEnumerable<object[]> Data
    {
        get
        {
            yield return new object[] { "", "1" };
            yield return new object[] { "a", "1" };
            yield return new object[] { "a1", "1" };
            yield return new object[] { "a1a", "1" };
        }
    }

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
}