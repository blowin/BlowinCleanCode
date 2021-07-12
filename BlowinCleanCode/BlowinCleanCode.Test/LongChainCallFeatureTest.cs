using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    public class LongChainCallFeatureTest
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
        // Disable BCC4002
        // Disable BCC2003
        class Test
        {
            public bool Run(IEnumerable<int> seq)
            {
                return {|#0:seq
                    .Select(i => i * i)
                    .Select(e => e.ToString())
                    .Select(e => 1)
                    .Select(e => e.ToString())
                    .Select(e => 1)
                    .Select(e => e.ToString())                    
                    .Where(e => e.Contains('1'))
                    .Any(e => e.Length > 0)|};
}
        }
    }", @"seq
                    .Select(i => i * i)
                    .Select(e => e.ToString())
                    .Select(e => 1)
                    .Select(e => e.ToString())
                    .Select(e => 1)
                    .Select(e => e.ToString())                    
                    .Where(e => e.Contains('1'))
                    .Any(e => e.Length > 0)")]
        public async Task LongChainCall(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.LongChainCall).WithLocation(0).WithArguments(argument);
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
        // Disable BCC4002
        // Disable BCC2003
        class Test
        {
            public bool Run(IEnumerable<int> seq)
            {
                return {|#0:seq
                    .Select(e => e.ToString())                    
                    .Where(e => e.Contains('1'))
                    .Where(e => e.Contains('1'))
                    .Where(e => e.Contains('1'))
                    .Where(e => e.Contains('1'))
                    .Where(e => e.Contains('1'))
                    .Any(e => e.Length > 0)|};
}
        }
    }")]
        public async Task LongChainCall_Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}