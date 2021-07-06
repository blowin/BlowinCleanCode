using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    [TestClass]
    public class ComplexConditionFeatureTest
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
        // Disable BCC2004
        // Disable BCC2005
        class Test
        {
            public bool Run(float price, bool eur)
            {
                var ttt2 = {|#0:price > 0 && price < 360 && eur || price != 360 || !eur|} ? eur : eur;
                return true;
            }
        }
    }", "price > 0 && price < 360 && eur || price != 360 || !eur")]
        [DataRow(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        // Disable BCC2004
        // Disable BCC2005
        class Test
        {
            public bool Run(float price, bool eur)
            {
                var ttt = {|#0:price > 0 && price < 360 && eur || price != 360 || Check1(price)|};
                return true;
            }

            public bool Check1(float price) => price > 0 || price < 1000;
        }
    }", "price > 0 && price < 360 && eur || price != 360 || Check1(price)")]
        [DataRow(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        // Disable BCC2004
        // Disable BCC2005
        class Test
        {
            public bool Run(float price, bool eur)
            {
                if({|#0:price > 0 && price < 360 && eur || price != 360 || !eur|}){
                    return Check1(price);
                }

                return true;
            }

            public bool Check1(float price) => price > 0 || price < 1000;
        }
    }", "price > 0 && price < 360 && eur || price != 360 || !eur")]
        [DataRow(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        // Disable BCC2004
        // Disable BCC2005
        class Test
        {
            public bool Run(float price, bool eur)
            {
                while({|#0:price > 0 && price < 360 && (eur && price != 360) || Check1(price)|}){
                }
                
                return true;
            }

            public bool Check1(float price) => price > 0 || price < 1000;
        }
    }", "price > 0 && price < 360 && (eur && price != 360) || Check1(price)")]
        [DataRow(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        // Disable BCC2004
        // Disable BCC2005
        class Test
        {
            public bool Run(float price, bool eur)
            {
                return {|#0:price > 0 && price < 360 && (eur && price != 360) || Check1(price)|};
            }

            public bool Check1(float price) => price > 0 || price < 1000;
        }
    }", "price > 0 && price < 360 && (eur && price != 360) || Check1(price)")]
        [DataRow(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        // Disable BCC2004
        // Disable BCC4002
        class Test
        {
            public void Run(float price, bool eur)
            {
                Go({|#0:price > 0 && price < 360 && (eur && price != 360) || Check1(price)|}, true);
            }

            public void Go(bool price, bool v2) {}

            public bool Check1(float price) => price > 0 || price < 1000;
        }
    }", "price > 0 && price < 360 && (eur && price != 360) || Check1(price)")]
        public async Task ComplexCondition(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.ComplexCondition).WithLocation(0).WithArguments(argument);
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
        // Disable BCC2004
        // Disable BCC2005
        class Test
        {
            public bool Run(float price, bool eur)
            {
                var ttt2 = price < 360 && eur || price != 360 ? eur : eur;
                return true;
            }
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
        // Disable BCC2004
        // Disable BCC2005
        class Test
        {
            public bool Run(float price, bool eur)
            {
                var ttt = price > 0 && price < 360 && eur;
                return true;
            }
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
        // Disable BCC2004
        // Disable BCC2005
        class Test
        {
            public bool Run(float price, bool eur)
            {
                if(price != 360 || !eur){
                    return Check1(price);
                }

                return true;
            }

            public bool Check1(float price) => price > 0 || price < 1000;
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
        // Disable BCC2004
        // Disable BCC2005
        class Test
        {
            public bool Run(float price, bool eur)
            {
                while(price > 0){
                }
                
                return true;
            }

            public bool Check1(float price) => price > 0 || price < 1000;
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
        // Disable BCC2004
        // Disable BCC2005
        class Test
        {
            public bool Run(float price, bool eur)
            {
                return (eur && price != 360) || Check1(price);
            }

            public bool Check1(float price) => price > 0 || price < 1000;
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
        // Disable BCC4002
        // Disable BCC2005
        class Test
        {
            public void Run(float price, bool eur)
            {
                Go(price > 0 && price < 360 || Check1(price), true);
            }

            public void Go(bool price, bool v2) {}

            public bool Check1(float price) => price > 0 || price < 1000;
        }
    }")]
        public async Task ComplexCondition_Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}