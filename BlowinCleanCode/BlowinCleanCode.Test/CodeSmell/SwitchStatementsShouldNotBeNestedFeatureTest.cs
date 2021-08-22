using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.CodeSmell
{
    public class SwitchStatementsShouldNotBeNestedFeatureTest
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
        // Disable BCC3003
        public class Calculator
        {
            void Run(string v){
                switch(v){
                    case ""v1"":
                        return;
                    case ""v2"":
                        return;
                    case ""v3"":
                        {|#0:switch(v){
                            case ""z"":
                                break;
                            default:
                                break;
                        }|}
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
        // Disable BCC3003
        public class Calculator
        {
            void Run(string v){
                switch(v){
                    case ""v1"":
                        {|#0:switch(v){
                            case ""z"":
                                break;
                            default:
                                break;
                        }|}
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
        // Disable BCC3003
        public class Calculator
        {
            void Run(string v){
                {
                    switch(v){
                        case ""v1"":
                            {|#0:switch(v){
                                case ""z"":
                                    break;
                                default:
                                    break;
                            }|}
                            return;
                        default:
                            return;
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
        // Disable BCC3003
        public class Calculator
        {
            void Run(IEnumerable<string> seq){
                {
                    var result = seq.Select(v => {
                        switch(v){
                            case ""v1"":
                                {|#0:switch(v){
                                    case ""z"":
                                        return 1;
                                    default:
                                        return 2;
                                }|}
                                return 3;
                            default:
                                return 4;
                        }
                    }).Sum();
                }
            }
        }
    }")]
        public async Task Invalid(string test)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.SwitchStatementsShouldNotBeNested).WithLocation(0);
            
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
        // Disable BCC3003
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
        // Disable BCC3003
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        // Disable BCC3003
        public class Calculator
        {
            void Run(string v){
                switch(v){
                    case ""v1"":
                        return;
                    case ""v2"":
                        Run2(v);
                        return;
                    default:
                        return;
                }
            }

            void Run2(string v)
            {
                switch(v)
                {
                    case ""zzz"":
                        break;
                    default:
                        break;
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
        // Disable BCC3003
        public class Calculator
        {
            void Run(string v){
                switch(v){
                    case ""v1"":
                    case ""v2"":
                        Run2(v);                        
                        return;
                    default:
                        return;
                }
            }

            void Run2(string v)
            {
                switch(v)
                {
                    case ""zzz"":
                        break;
                    default:
                        break;
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