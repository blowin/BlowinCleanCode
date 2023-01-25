using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.GoodPractice
{
    public class NameTooLongFeatureTest
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
                // Disable BCC3007
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
                // Disable BCC3007
                string iAmAVeryLongNamePleaseShortenMe = null;
                Dummy(({|#0:iAmAVeryLongNamePleaseShortenMe|} + """"));
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
                // Disable BCC3007
                var iAmAVeryLongNamePleaseShortenMe = """";
                var len = {|#0:iAmAVeryLongNamePleaseShortenMe|}.Length;
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
            private string {|#0:_iAmAVeryLongNamePleaseShortenMe|} = null;
        }
    }", "_iAmAVeryLongNamePleaseShortenMe")]
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
            private string {|#0:IAmAVeryLongNamePleaseShortenMe|} { get; set; }
        }
    }", "IAmAVeryLongNamePleaseShortenMe")]
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
            void {|#0:IAmAVeryLongNamePleaseShortenMe|}(){
            }
        }
    }", "IAmAVeryLongNamePleaseShortenMe")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public struct  {|#0:IAmAVeryLongNamePleaseShortenMe|}
        {
        }
    }", "IAmAVeryLongNamePleaseShortenMe")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public struct  Calculator
        {
            public event Action {|#0:IAmAVeryLongNamePleaseShortenMe|};
        }
    }", "IAmAVeryLongNamePleaseShortenMe")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        public struct  Calculator
        {
            public delegate int {|#0:IAmAVeryLongNamePleaseShortenMe|}(string value);
        }
    }", "IAmAVeryLongNamePleaseShortenMe")]
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
            private const int {|#0:_iAmAVeryLongNamePleaseShortenMe|} = 1;
        }
    }", "_iAmAVeryLongNamePleaseShortenMe")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        // Disable BCC3007
        public class Holder
        {
            public class  IAmAVeryLongNamePleaseShortenMe
            {
                public const string Word = ""Hi"";
            }
        }

        public class Program
        {
            public string Get(string value) => value;

            public string Run()
            {
                var result = Get(Holder.{|#0:IAmAVeryLongNamePleaseShortenMe|}.Word);
                return result;
            }
        }
    }", "IAmAVeryLongNamePleaseShortenMe")]
        public async Task Invalid(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.NameTooLong).WithLocation(0).WithArguments(argument);
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
                var shortName = """";
                var len = shortName.Length;
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
            void PrintImmOrderReport(){
            }
        }
    }")]
        public async Task Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
