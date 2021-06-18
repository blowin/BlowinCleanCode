using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    [TestClass]
    public class PublicStaticFieldFeatureTest
    {
        [TestMethod]
        public async Task Public_Static_Field()
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
        class Test
        {   
            public static int {|#0:Value|};
        }
    }";
            var expected = VerifyCS.Diagnostic(Constant.Id.PublicStaticField).WithLocation(0).WithArguments("Value");
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
        public enum Level
        {
            Debug,
            Info,
            Warning,
            Error
        }
    }")]
        [DataRow(@"
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
            public const string App = ""Analyzer"";
        }
    }")]
        public async Task Public_Static_Field_Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}