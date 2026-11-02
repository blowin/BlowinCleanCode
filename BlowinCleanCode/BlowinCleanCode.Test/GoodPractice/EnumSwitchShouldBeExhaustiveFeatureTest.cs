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
    public async Task GuardedLabelsDoNotCountAsExhaustiveCoverage()
    {
        const string test = @"
enum OrderStatus { Pending, Processing, Shipped }

class OrderHandler
{
    bool IsAvailable() => true;

    void Handle(OrderStatus status)
    {
        {|#0:switch|} (status)
        {
            case OrderStatus.Pending:
                break;
            case OrderStatus.Processing:
                break;
            case OrderStatus.Shipped when IsAvailable():
                break;
        }
    }

    string GetValue(OrderStatus status)
    {
        return status {|#1:switch|}
        {
            OrderStatus.Pending => nameof(OrderStatus.Pending),
            OrderStatus.Processing => nameof(OrderStatus.Processing),
            OrderStatus.Shipped when IsAvailable() => nameof(OrderStatus.Shipped),
            _ => nameof(OrderStatus.Pending)
        };
    }
}";

        var statementExpected = VerifyCS.Diagnostic(Constant.Id.EnumSwitchShouldBeExhaustive)
            .WithLocation(0)
            .WithArguments("Shipped");
        var expressionExpected = VerifyCS.Diagnostic(Constant.Id.EnumSwitchShouldBeExhaustive)
            .WithLocation(1)
            .WithArguments("Shipped");
        await VerifyCS.VerifyAnalyzerAsync(test, statementExpected, expressionExpected);
    }

    [Fact]
    public async Task FlagsEnumsAreIgnoredForBothSwitchForms()
    {
        const string test = @"
using System;

[Flags]
enum Permission { None = 0, Read = 1, Write = 2 }

class Handler
{
    void Check(Permission permission)
    {
        switch (permission)
        {
            case Permission.Read:
                break;
            case Permission.Write:
                break;
        }
    }

    int GetValue(Permission permission)
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

    [Fact]
    public async Task AliasedEnumValuesNeedOnlyOneLabelPerUnderlyingValue()
    {
        const string test = @"
enum State { Initial = 0, Alias = 0, Complete = 1 }

class Handler
{
    int GetValue(State state)
    {
        return state switch
        {
            State.Initial => 0,
            State.Complete => 1
        };
    }
}";

        await VerifyCS.VerifyAnalyzerAsync(test);
    }

    [Fact]
    public async Task NestedAndAdjacentSwitchesAreCheckedIndependently()
    {
        const string test = @"
// Disable BCC3003
// Disable BCC4002
enum State { New, Active, Closed }
enum Stage { Started, Running, Finished }

class Handler
{
    // Disable BCC2009
    string Handle(State state, Stage stage)
    {
        // Disable BCC4008
        {|#0:switch|} (state)
        {
            case State.New:
                {|#1:switch|} (stage)
                {
                    case Stage.Started:
                        break;
                    case Stage.Running:
                        break;
                }
                break;
            case State.Active:
                break;
        }

        switch (state)
        {
            case State.New:
                break;
            case State.Active:
                break;
            case State.Closed:
                break;
        }

        return stage {|#2:switch|}
        {
            Stage.Started => nameof(Stage.Started),
            Stage.Running => nameof(Stage.Running),
            _ => nameof(Stage.Finished)
        };
    }
}";

        var outerExpected = VerifyCS.Diagnostic(Constant.Id.EnumSwitchShouldBeExhaustive)
            .WithLocation(0)
            .WithArguments("Closed");
        var nestedExpected = VerifyCS.Diagnostic(Constant.Id.EnumSwitchShouldBeExhaustive)
            .WithLocation(1)
            .WithArguments("Finished");
        var expressionExpected = VerifyCS.Diagnostic(Constant.Id.EnumSwitchShouldBeExhaustive)
            .WithLocation(2)
            .WithArguments("Finished");

        await VerifyCS.VerifyAnalyzerAsync(test, outerExpected, nestedExpected, expressionExpected);
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
    
        [Fact]
    public async Task GuardedLabelsStrictlyFailCoveringTheValue()
    {
        // Реальный тест на баг текущей реализации:
        // Анализатор извлекает 'Pending' из 'OrderStatus.Pending when IsAvailable()'
        // и считает его полностью покрытым, хотя при IsAvailable() == false ветка пропустится!
        const string test = @"
enum OrderStatus { Pending, Processing, Shipped }

class OrderHandler
{
    bool IsAvailable() => true;

    void Handle(OrderStatus status)
    {
        {|#0:switch|} (status)
        {
            case OrderStatus.Pending when IsAvailable():
                break;
            case OrderStatus.Processing:
                break;
            case OrderStatus.Shipped:
                break;
        }
    }
}";

        var expected = VerifyCS.Diagnostic(Constant.Id.EnumSwitchShouldBeExhaustive)
            .WithLocation(0)
            .WithArguments("Pending");
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task TypePatternsAreIgnoredOrCountedCorrectly()
    {
        const string test = @"
enum OrderStatus { Pending, Processing }
// Disable BCC3003
class OrderHandler
{
    void Handle(OrderStatus status)
    {
        {|#0:switch|} (status)
        {
            case OrderStatus currentStatus:
                break;
        }
    }

    int GetValue(OrderStatus status)
    {
        return status switch
        {
            OrderStatus _ => 1
        };
    }
}";

        var expected = VerifyCS.Diagnostic(Constant.Id.EnumSwitchShouldBeExhaustive)
            .WithLocation(0)
            .WithArguments("Pending, Processing");
            
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Fact]
    public async Task SwitchByNonEnumTypesIsIgnored()
    {
        const string test = @"
class Handler
{
    void Handle(int value)
    {
        switch (value)
        {
            case 1: break;
            case 2: break;
        }
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}
