using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.GoodPractice;

public class EnumSwitchShouldBeExhaustiveFeatureTest
{
    [Fact]
    public async Task SwitchStatementReportsMembersMissingDespiteDefault()
    {
        const string test = @"
enum OrderStatus { Pending, Processing, Shipped }

class OrderHandler
{
    void Handle(OrderStatus status)
    {
        {|#0:switch|} (status)
        {
            case OrderStatus.Pending:
                break;
            case OrderStatus.Processing:
                break;
            default:
                throw new System.ArgumentOutOfRangeException();
        }
    }
}";

        var expected = VerifyCS.Diagnostic(Constant.Id.EnumSwitchShouldBeExhaustive)
            .WithLocation(0)
            .WithArguments("Shipped");
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task SwitchExpressionReportsMissingMembersDespiteDiscardArm()
    {
        const string test = @"
enum OrderStatus { Pending, Processing, Shipped }

class OrderHandler
{
    int GetValue(OrderStatus status)
    {
        return status {|#0:switch|}
        {
            OrderStatus.Pending or OrderStatus.Processing => 0,
            _ => -1
        };
    }
}";

        var expected = VerifyCS.Diagnostic(Constant.Id.EnumSwitchShouldBeExhaustive)
            .WithLocation(0)
            .WithArguments("Shipped");
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task CompleteAndFlagsSwitchesAreValid()
    {
        const string test = @"
using System;

enum OrderStatus { Pending, Processing, Shipped }
[Flags]
enum Permission { Read = 1, Write = 2 }

class Handler
{
    string GetValue(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Pending => nameof(OrderStatus.Pending),
            OrderStatus.Processing => nameof(OrderStatus.Processing),
            OrderStatus.Shipped => nameof(OrderStatus.Shipped)
        };
    }

    int GetPermissionValue(Permission permission)
    {
        return permission switch
        {
            Permission.Read => 1,
            _ => 0
        };
    }
}";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}
