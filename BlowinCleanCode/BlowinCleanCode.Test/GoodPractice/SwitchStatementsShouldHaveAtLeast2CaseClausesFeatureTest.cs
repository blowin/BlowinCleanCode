using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.GoodPractice
{
    public class SwitchStatementsShouldHaveAtLeast2CaseClausesFeatureTest
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
        public class Calculator
        {
            void Run(string v){
                {|#0:switch(v){
                    case ""v1"":
                        return;
                    default:
                        return;
                }|}
            }
        }
    }")]
        public async Task Invalid(string test)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.SwitchStatementsShouldHaveAtLeast2CaseClauses).WithLocation(0);
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
        public class Calculator
        {
            void Run(string v){
                switch(v){
                    case ""v1"":
                        return;
                    case ""v2"":
                        return;
                    default:
                        return;
                }
            }
        }
    }")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class Calculator
        {
            void Run(string v){
                switch(v){
                    case ""v1"":
                    case ""v2"":
                        return;
                    default:
                        return;
                }
            }
        }
    }")]
        [InlineData(@"
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public class Test
        {
            // Disable BCC4006
            // Disable BCC4007
            public IEnumerable<int> ObjectAsIntEnumerable(object val){
                switch(val){
                    case int intV:
                        return new[] {intV};
                    case string str when !string.IsNullOrEmpty(str):
                        return str.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                    case IEnumerable<int> c:
                        return c;
                    case IEnumerable c:
                        return c.Cast<int>();
                    default:
                        return Enumerable.Empty<int>();
                }
            }
        }
    }")]
        public async Task Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}