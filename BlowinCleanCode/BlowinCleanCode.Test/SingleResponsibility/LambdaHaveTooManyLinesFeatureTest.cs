using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.SingleResponsibility
{
    public class LambdaHaveTooManyLinesFeatureTest
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
        public class Test
        {
            public int Run(int[] data)
            {
                return data.Select({|#0:e =>
                {
                    var v1 = e;
                    var v2 = e;
                    var v3 = e;
                    var v4 = e;
                    var v5 = e;
                    var v6 = e;
                    var v7 = e;
                    var v8 = e;
                    var v9 = e;
                    var v10 = e;
                    return e;
                }|}).Sum();
            }
        }
    }")]
        public async Task Invalid(string test)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.LongLambda).WithLocation(0);
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
        public class Test
        {
            public int Run(int[] data)
            {
                return data.Select(e =>
                {
                    var v1 = e;
                    var v2 = e;
                    var v3 = e;
                    return e;
                }).Sum();
            }
        }
    }")]
        public async Task Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}