using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.CodeSmell
{
    public class VariableNameTooLongFeatureTest
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
            void Run(){
                string {|#0:iAmAVeryLongNamePleaseShortenMe|} = null;
            }
        }
    }", "iAmAVeryLongNamePleaseShortenMe")]
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
            void Run(string {|#0:iAmAVeryLongNamePleaseShortenMe|}){
            }
        }
    }", "iAmAVeryLongNamePleaseShortenMe")]
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
            void Run(){
                // Disable BCC4012
                string iAmAVeryLongNamePleaseShortenMe = null;
                Dummy({|#0:iAmAVeryLongNamePleaseShortenMe|});
            }

            void Dummy(string value){}
        }
    }", "iAmAVeryLongNamePleaseShortenMe")]
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
            void Run(){
                // Disable BCC4012
                string iAmAVeryLongNamePleaseShortenMe = null;
                Dummy(({|#0:iAmAVeryLongNamePleaseShortenMe|} + """"));
            }

            void Dummy(string value){}
        }
    }", "iAmAVeryLongNamePleaseShortenMe")]
        public async Task Invalid(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.VariableNameTooLong).WithLocation(0).WithArguments(argument);
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
            void Run(){
                string shortName = null;
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
            void Run(string shortName){
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
            void Run(){
                string shortName = null;
                Dummy(shortName);
            }

            void Dummy(string value){}
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
            void Run(){
                string shortName = null;
                Dummy(shortName + """");
            }

            void Dummy(string value){}
        }
    }")]
        public async Task Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}