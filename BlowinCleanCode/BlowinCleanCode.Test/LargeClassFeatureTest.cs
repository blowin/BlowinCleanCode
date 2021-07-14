using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    public class LargeClassFeatureTest
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
        class {|#0:Test|}
        {   
            public static void Run1(){}
            public static void Run2(){}
            public static void Run3(){}
            public static void Run4(){}
            public static void Run5(){}
            public static void Run6(){}
            public static void Run7(){}
            public static void Run8(){}
            public static void Run9(){}
            public static void Run10(){}
            static void Run11(){}
        }
    }", "Test")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class {|#0:Test|}
        {   
            public static void Run1(){}
            public static void Run2(){}
            public static void Run3(){}
            public static void Run4(){}
            public static void Run5(){}
            public static void Run6(){}
            public static void Run7(){}
            public static void Run8(){}
            public static void Run9(){}
            static void Run11(){}
            static void Run12(){}
            static void Run13(){}
        }
    }", "Test")]
        public async Task Invalid(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.LargeClass).WithLocation(0).WithArguments(argument);
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
        class {|#0:Test|}
        {   
            public static void Run1(){}
            public static void Run2(){}
            public static void Run3(){}
            public static void Run4(){}
            public static void Run5(){}
            public static void Run6(){}
            public static void Run7(){}
            public static void Run8(){}
            public static void Run9(){}
            public static void Run10(){}
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
        class {|#0:Test|}
        {   
            static void Run1(){}
            static void Run2(){}
            static void Run3(){}
            static void Run4(){}
            static void Run5(){}
            static void Run6(){}
            static void Run7(){}
            static void Run8(){}
            static void Run9(){}
            static void Run10(){}
            static void Run11(){}
            static void Run12(){}
            static void Run13(){}
            static void Run14(){}
            static void Run15(){}
            static void Run16(){}
            static void Run17(){}
            static void Run18(){}
            static void Run19(){}
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
        class {|#0:Test|}
        {   
            public static void Run1(){}
            public static void Run2(){}
            public static void Run3(){}
            public static void Run4(){}
            public static void Run5(){}
            public static void Run6(){}
            public static void Run7(){}
            public static void Run8(){}
            public static void Run9(){}
            static void Run11(){}
            static void Run12(){}
        }
    }")]
        public async Task Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}