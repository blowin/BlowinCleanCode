using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    public class ReturnNullFeatureTest
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
        class Test
        {
            // Disable BCC4002
            public string Calculate(int age)
            {
                if(age > 18)
                {
                    var retV = ""Oh my)"";
                    return retV; 
                }

                return {|#0:null|};
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
        class Test
        {
            public string Calculate()
            {
                return {|#0:null|};
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
        class Test
        {
            public string Calculate() => {|#0:null|};
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
        class Test
        {
            // Disable BCC4002
            public string Calculate(int age)
            {
                if(age > 18)
                    return {|#0:null|};

                var retV = ""Oh my)"";
                return retV;
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
        class Test
        {
            // Disable BCC4002
            public string Calculate(int age)
            {
                return age > 18 ? {|#0:null|} : ""Oh my)"";
            }
        }
    }")]
        public async Task Invalid(string test)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.ReturnNull).WithLocation(0);
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
        class Test
        {
            public string Calculate(int age)
            {
                return BuildStr(null);
            }

            private string BuildStr(string str) => ""test"";
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
        // Disable BCC4002
        class Test
        {
            public string Calculate(int age)
            {
                return age > 18 ? BuildStr(null) : ""ops"";
            }

            private string BuildStr(string str) => ""test"";
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
        class Test
        {
            public void Calculate()
            {
                return;
            }
        }
    }")]
        public async Task Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}