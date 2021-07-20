using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    public class ControlFlagFeatureTest
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
            public float Run(float price, bool eur)
            {
                return {|#0:eur|} ? CalculateEur(price) : CalculateDefault(price);
            }

            public float CalculateEur(float price) => price;
            
            public float CalculateDefault(float price) => price;
        }
    }", "eur")]
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
            public float Run(float price, bool eur) => {|#0:eur|} ? CalculateEur(price) : CalculateDefault(price);

            public float CalculateEur(float price) => price;
            
            public float CalculateDefault(float price) => price;
        }
    }", "eur")]
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
            public float Run(float price, bool eur)
            {
                if({|#0:eur|}){
                    return CalculateEur(price);
                }
                
                return CalculateDefault(price);
            }

            public float CalculateEur(float price) => price;
            
            public float CalculateDefault(float price) => price;
        }
    }", "eur")]
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
            public void Run(float price, bool eur)
            {
                if({|#0:eur|}){
                    CalculateEur(price);
                    return;
                }
                
                CalculateDefault(price);
            }

            public void CalculateEur(float price){}
            
            public void CalculateDefault(float price){}
        }
    }", "eur")]
        public async Task Invalid(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.ControlFlag).WithLocation(0).WithArguments(argument);
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
            public bool Run(bool dummy)
            {
                return dummy ? true : false;
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
        // Disable BCC4002
        class Test
        {
            public bool Run(bool dummy) => dummy ? true : false;
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
            public bool Run(bool dummy)
            {
                if(dummy){
                    return true;
                }
                
                return false;
            }

            public float CalculateEur(float price) => price;
            
            public float CalculateDefault(float price) => price;
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
            public void Run(bool dummy)
            {
                if(dummy)
                    return;
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
            public void Run(bool dummy)
            {
                if(dummy){
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
