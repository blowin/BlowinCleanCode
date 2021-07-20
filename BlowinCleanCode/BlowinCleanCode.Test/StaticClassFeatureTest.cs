using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    public class StaticClassFeatureTest
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
        // Disable BCC4002
        public static class {|#0:Calculator|}
        {
            static void Main(string[] args){}

            public static string Calculate(int age)
            {
                if(age > 18)
                    return age.ToString();

                return age.ToString();
            }
            
            public static string FormatInt(this int self) => self.ToString();
        }
    }", "Calculator")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public static class {|#0:Calculator2|}
        {
            public static int Identity(int v) => v;
            
            public static string FormatInt(this int self) => self.ToString();

            public static string FormatInt2(this int self)
            {
                return self.ToString();
            }
        }
    }", "Calculator2")]
        public async Task Class_Can_Not_Be_Static(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.StaticClass).WithLocation(0).WithArguments(argument);
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
        public static class Calculator
        {
            static void Main(string[] args){}
        }
    }")]
        public async Task Class_Can_Not_Be_Static_Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}