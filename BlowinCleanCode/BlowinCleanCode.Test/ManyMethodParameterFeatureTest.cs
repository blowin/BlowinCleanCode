using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    [TestClass]
    public class ManyMethodParameterFeatureTest
    {
        [TestMethod]
        public async Task Many_Method_Parameters()
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
        class TEST
        {   
            public static void {|#0:Run|}(int i, int i2, int i3, int i4, int i5)
            {

            }
        }
    }";
            var expected = VerifyCS.Diagnostic(Constant.Id.ManyParametersMethod).WithLocation(0).WithArguments("Run");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}