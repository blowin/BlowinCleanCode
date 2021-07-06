using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    [TestClass]
    public class NestedTernaryOperatorFeatureTest
    {
        [TestMethod]
        [DataRow(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TEST
        {   
            // Disable BCC2003
            // Disable BCC4002
            public static string Run(bool flag1, bool flag2)
            {
                return flag1 ? ({|#0:flag2 ? ""1"" : ""2""|}) : ""3"";
            }
        }
    }", @"flag2 ? ""1"" : ""2""")]
        [DataRow(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TEST
        {   
            // Disable BCC2003
            // Disable BCC4002
            public static string Run(bool flag1, bool flag2, IDisposable d)
            {
                using(d)
                    return flag1 ? ({|#0:flag2 ? ""1"" : ""2""|}) : ""3"";
            }
        }
    }", @"flag2 ? ""1"" : ""2""")]
        public async Task Invalid(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.NestedTernaryOperator).WithLocation(0).WithArguments(argument);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
        
        [TestMethod]
        [DataRow(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TEST
        {   
            // Disable BCC2003
            // Disable BCC4002
            public static string Run(bool flag1, bool flag2)
            {
                return flag1 ? ""1"" : ""2"";
            }
        }
    }")]
        public async Task Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}