using System.Threading.Tasks;
using BlowinCleanCode.Model;
using FluentAssertions;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.SingleResponsibility;

public class CognitiveComplexityMethodFeatureTest
{
    [Theory]
    [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        using System;
using System.Threading.Tasks;

public class A
{
    // Disable BCC2003
    // Disable BCC2009
    // Disable BCC2004
    // Disable BCC4002
    public void {|#0:Run|}(bool b)
    {
        if (true)                       // +1
        {
            if (b)                      // +2 (N=1)
                Console.WriteLine();
            else if (!b)                // +2 (N=1)
                Console.WriteLine();
            else                        // +2 (N=1)
                Console.WriteLine();
        }

        if (!b)                         // +1
            Console.WriteLine();
    }
}
    }", 8)]
    [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        using System;
using System.Threading.Tasks;

public class A
{
    // Disable BCC2003
    // Disable BCC2009
    // Disable BCC2004
    // Disable BCC4002
    public void {|#0:Run|}(bool b)
    {
        if (true)                       // +1
        {
            if (b)                      // +2 (N=1)
                Console.WriteLine();
            else if (!b)                // +2 (N=1)
                Console.WriteLine();
            else if (b)                 // +2 (N=1)
                Console.WriteLine();
            else                        // +2 (N=1)
                Console.WriteLine();
        }

        if (!b)                         // +1
            Console.WriteLine();
    }
}
    }", 10)]
    [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        using System;
using System.Threading.Tasks;

public class A
{
    // Disable BCC2003
    // Disable BCC2009
    // Disable BCC2004
    // Disable BCC4002
    public void {|#0:Run|}(bool b)
    {
        if (true)                       // +1
        {
            if (b)                      // +2 (N=1)
                Console.WriteLine();
            else if (!b)                // +2 (N=1)
                Console.WriteLine();
            else                        // +2 (N=1)
                Console.WriteLine();
        }

        if (!b)                         // +1
            Console.WriteLine();

        if (true)                       // +1
        {
            if (b)                      // +2 (N=1)
                Console.WriteLine();
            else if (!b)                // +2 (N=1)
                Console.WriteLine();
            else                        // +2 (N=1)
                Console.WriteLine();
        }

        if (!b)                         // +1
            Console.WriteLine();
    }
}
    }", 16)]

    public async Task Invalid(string test, int actualComplexity)
    {
        var settings = new CognitiveComplexitySettings();

        var tryBuildMessage = settings.TryBuildMessage(actualComplexity, out var res);

        var expected = VerifyCS.Diagnostic(Constant.Id.CognitiveComplexity)
            .WithLocation(0)
            .WithArguments("Run", res);

        tryBuildMessage.Should().BeTrue();
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [Theory]
    [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class Test
        {
            public static int Run(List<int> seq)
            {
                var sum = seq
                .OrderBy(e => e)
                .Select(e =>
                {
                    var str = e.ToString();
                    var strLen = str.Length;
                    var curValue = e;
                    return strLen * curValue;
                })
                .AsParallel()
                .AsUnordered()
                .Sum();
            
                var sum2 = seq
                    .OrderBy(e => e)
                    .Select(e => e.ToString().Length)
                    .AsParallel()
                    .AsUnordered()
                    .Sum();

                var result = sum + sum2;
                return result;
            }
        }
    }")]
    public async Task Valid(string test)
    {
        await VerifyCS.VerifyAnalyzerAsync(test);
    }
}