using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.SingleResponsibility
{
    public class LargeNumberOfFieldsFeatureTest
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
        struct {|#0:Test|}
        {
            private int _age1;
            private int _age2;
            private int _age3;
            public int Age4 { get; set; }
            public int Age5 { get; set; }
            public int Age6 { get; set; }

            public void Run(){}
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
        struct {|#0:Test|}
        {   
            private int _age1;
            private int _age2;
            private int _age3;
            private int _age4;
            private int _age5;
            private int _age6;

            public void Run(){}
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
        struct {|#0:Test|}
        { 
            public int Age1 { get; set; }
            public int Age2 { get; set; }
            public int Age3 { get; set; }
            public int Age4 { get; set; }
            public int Age5 { get; set; }
            public int Age6 { get; set; }

            public void Run(){}
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
            private int _age1;
            private int _age2;
            private int _age3;
            public int Age4 { get; set; }
            public int Age5 { get; set; }
            public int Age6 { get; set; }

            public void Run(){}
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
            private int _age1;
            private int _age2;
            private int _age3;
            private int _age4;
            private int _age5;
            private int _age6;

            public void Run(){}
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
            public int Age1 { get; set; }
            public int Age2 { get; set; }
            public int Age3 { get; set; }
            public int Age4 { get; set; }
            public int Age5 { get; set; }
            public int Age6 { get; set; }

            public void Run(){}
        }
    }", "Test")]
        public async Task Invalid(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.LargeNumberOfFields).WithLocation(0).WithArguments(argument);
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
        class Manager
        { 
            public int Age1 { get; set; }
            public int Age2 { get; set; }

            public void Run(){}
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
            public int Age { get; set; }

            public void Run(){}
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
        struct Manager
        { 
            public int Age1 { get; set; }
            public int Age2 { get; set; }

            public void Run(){}
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
        struct Test
        {
            public int Age { get; set; }

            public void Run(){}
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
        struct Test
        { 
            public int Age1 { get; set; }
            public int Age2 { get; set; }
            public int Age3 { get; set; }
            public int Age4 { get; set; }
            public int Age5 { get; set; }
            public int Age6 { get; set; }
            public int Age7 { get; set; }
            public int Age8 { get; set; }
            public int Age9 { get; set; }
            public int Age10 { get; set; }
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
            public int Age1 { get; set; }
            public int Age2 { get; set; }
            public int Age3 { get; set; }
            public int Age4 { get; set; }
            public int Age5 { get; set; }
            public int Age6 { get; set; }
            public int Age7 { get; set; }
            public int Age8 { get; set; }
            public int Age9 { get; set; }
            public int Age10 { get; set; }
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
            public int Age1 { get; set; }
            public int Age2 { get; set; }
            public int Age3 { get; set; }
            public int Age4 { get; set; }
            public int Age5 { get; set; }
            public int Age6 { get; set; }
            public int Age7 { get; set; }
            public int Age8 { get; set; }
            public int Age9 { get; set; }
            public int Age10 { get; set; }

            public override string ToString()
            {
                return Age1.ToString();
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
            public int Age1 { get; set; }
            public int Age2 { get; set; }
            public int Age3 { get; set; }
            public int Age4 { get; set; }
            public int Age5 { get; set; }
            public int Age6 { get; set; }
            public int Age7 { get; set; }
            public int Age8 { get; set; }
            public int Age9 { get; set; }
            public int Age10 { get; set; }

            public override string ToString() => Age1.ToString();
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
        struct Test
        { 
            public int Age1 { get; set; }
            public int Age2 { get; set; }
            public int Age3 { get; set; }
            public int Age4 { get; set; }
            public int Age5 { get; set; }
            public int Age6 { get; set; }
            public int Age7 { get; set; }
            public int Age8 { get; set; }
            public int Age9 { get; set; }
            public int Age10 { get; set; }

            public override string ToString()
            {
                return Age1.ToString();
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
        struct Test
        { 
            public int Age1 { get; set; }
            public int Age2 { get; set; }
            public int Age3 { get; set; }
            public int Age4 { get; set; }
            public int Age5 { get; set; }
            public int Age6 { get; set; }
            public int Age7 { get; set; }
            public int Age8 { get; set; }
            public int Age9 { get; set; }
            public int Age10 { get; set; }

            public override string ToString() => Age1.ToString();
        }
    }")]
        public async Task Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}