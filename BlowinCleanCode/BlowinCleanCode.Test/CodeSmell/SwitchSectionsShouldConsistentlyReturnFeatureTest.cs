using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.CodeSmell;

public class SwitchSectionsShouldConsistentlyReturnFeatureTest
{
    [Fact]
    public async Task ReportsWhenOneSectionReturnsAndAnotherBreaks()
    {
        const string test = @"
// Disable BCC3003
// Disable BCC4010
class Test
{
    int Get(int value)
    {
        {|#0:switch|}(value)
        {
            case 0:
                return 0;
            case 1:
                break;
            default:
                throw new System.ArgumentException();
        }

        return -1;
    }
}";

        var expected = VerifyCS.Diagnostic(Constant.Id.SwitchSectionsShouldConsistentlyReturn)
            .WithLocation(0);

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task ReportsWhenReturnDoesNotCoverEveryPathInSection()
    {
        const string test = @"
// Disable BCC3003
// Disable BCC4010
class Test
{
    int Get(int value)
    {
        {|#0:switch|}(value)
        {
            case 0:
                if (value > 0)
                    return 0;
                break;
            default:
                return 1;
        }

        return -1;
    }
}";

        var expected = VerifyCS.Diagnostic(Constant.Id.SwitchSectionsShouldConsistentlyReturn)
            .WithLocation(0);

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Theory]
    [InlineData(@"
// Disable BCC3003
// Disable BCC4010
class Test
{
    int Get(int value)
    {
        switch (value)
        {
            case 0:
                return 0;
            case 1:
                if (value > 0)
                    return 1;
                else
                    throw new System.ArgumentException();
            default:
                throw new System.ArgumentException();
        }
    }
}")]
    [InlineData(@"
// Disable BCC3003
// Disable BCC4010
class Test
{
    int Get(int value)
    {
        switch (value)
        {
            case 0:
                value++;
                break;
            default:
                break;
        }

        return value;
    }
}")]
    [InlineData(@"
// Disable BCC3003
// Disable BCC4010
class Test
{
    int Get(int value)
    {
        switch (value)
        {
            case 0:
                int Local() => value;
                break;
            default:
                break;
        }

        return value;
    }
}")]
    [InlineData(@"
// Disable BCC3003
// Disable BCC4010
class Test
{
    int Get(int value)
    {
        switch (value)
        {
            case 0:
                try
                {
                    return 0;
                }
                finally
                {
                    Cleanup();
                }
            default:
                throw new System.ArgumentException();
        }
    }

    void Cleanup() { }
}")]
    [InlineData(@"
// Disable BCC3003
// Disable BCC4010
class Test
{
    int Get(int value) => value switch
    {
        0 => 0,
        _ => throw new System.ArgumentException()
    };
}")]
    public async Task DoesNotReportConsistentSwitches(string test)
    {
        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}
