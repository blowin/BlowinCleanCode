using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.CodeSmell
{
    public class EmptyDefaultClausesShouldBeRemovedFeatureTest
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
                switch(v){
                    case ""v1"":
                        return;
                    case ""v2"":
                        return;
                    default:
                        {|#0:break;|}
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
                        {|#0:break;|}
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
        // Disable BCC4006
        public class Calculator
        {
            void Run(string v){
                switch(v){
                    case ""v1"":
                        return;
                    case ""v2"":
                        return;
                    default:
                    {
                        {|#0:break;|}
                    }
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
                    {
                        {|#0:break;|}
                    }
                }
            }
        }
    }")]
        public async Task Invalid(string test)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.EmptyDefaultClausesShouldBeRemoved).WithLocation(0);

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
