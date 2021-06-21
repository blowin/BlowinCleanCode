using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    [TestClass]
    public class ControlFlagFeatureTest
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
        class Test
        {
            public float Run(float price, bool eur)
            {
                return {|#0:eur|} ? CalculateEur(price) : CalculateDefault(price);
            }

            public float CalculateEur(float price) => price;
            
            public float CalculateDefault(float price) => price;
        }
    }", "eur")]/*
        [DataRow(@"
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
    }", "eur")]*/
        public async Task Control_Flag(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.ControlFlag).WithLocation(0).WithArguments(argument);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
