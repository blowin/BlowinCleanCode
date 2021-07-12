using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    public class BlowinCleanCodeUnitTest
    {
        [Fact]
        public async Task Empty_Input_Verify_Analyzer()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task Without_Problem_Program_Verify_Analyzer()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class Calculator
        {   
            public int {|#0:Sum|}(int v1, int v2) => v1 + v2;
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
