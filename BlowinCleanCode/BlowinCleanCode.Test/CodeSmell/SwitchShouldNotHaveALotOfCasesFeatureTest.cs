using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.CodeSmell
{
    public class SwitchShouldNotHaveALotOfCasesFeatureTest
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
        // Disable BCC4006
        public class Calculator
        {
            void Run(string v){
                {|#0:switch(v){
                    case ""v1"":
                        return;
                    case ""v2"":
                        return;
                    case ""v3"":
                        return;
                    case ""v4"":
                        return;
                    case ""v5"":
                        return;
                    default:
                        return;
                }|}
            }
        }
    }", 5)]
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
                    case ""v2"":
                    case ""v3"":
                    case ""v4"":
                    case ""v5"":
                        return;
                    default:
                        return;
                }|}
            }
        }
    }", 5)]
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
            public IEnumerable<int> ObjectAsIntEnumerable(object val){
                {|#0:switch(val){
                    case int intV:
                        return new[] {intV};
                    case string str when !string.IsNullOrEmpty(str):
                        return str.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
                    case IEnumerable<int> c:
                        return c;
                    case IEnumerable c:
                        return c.Cast<int>();
                    case null:
                    default:
                        return Enumerable.Empty<int>();
                }|}
            }
        }
    }", 5)]
        public async Task Invalid(string test, int countOfCases)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.SwitchShouldNotHaveALotOfCases)
                .WithLocation(0)
                .WithArguments(countOfCases, AnalyzerSettings.Instance.MaxSwitchCaseCount);
            
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
                    case ""v3"":
                        return;
                    default:
                        return;
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