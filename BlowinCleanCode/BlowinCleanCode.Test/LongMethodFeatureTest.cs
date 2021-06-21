using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    [TestClass]
    public class LongMethodFeatureTest
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
            public static void {|#0:Run|}()
            {
                var i1 = 10;
                var i2 = 10;
                var i3 = 10;
                var i4 = 10;
                var i5 = 10;
                var i6 = 10;
                var i7 = 10;
                var i8 = 10;
                var i9 = 10;
                var i10 = 10;
                var i11 = 10;
                var i12 = 10;
                var i13 = 10;
                var i14 = 10;
                var i15 = 10;
                var i16 = 10;
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
        class Test
        {   
            public static void {|#0:Run|}(IDisposable obj)
            {
                using(obj)
                {
                    var i1 = 10;
                    var i2 = 10;
                    var i3 = 10;
                    var i4 = 10;
                    var i5 = 10;
                    var i6 = 10;
                    var i7 = 10;
                    var i8 = 10;
                    var i9 = 10;
                    var i10 = 10;
                    var i11 = 10;
                    var i12 = 10;
                    var i13 = 10;
                    var i14 = 10;
                    var i15 = 10;
                }
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
        class Test
        {   
            public static void Invoke(){}

            public static void {|#0:Run|}(IDisposable obj)
            {
                Invoke();
                using(obj)
                {
                    {
                        var i1 = 10;
                        var i2 = 10;
                        var i3 = 10;
                        var i4 = 10;
                    }
                    var i5 = 10;
                    var i6 = 10;
                    if (i5 != i6)
                    {
                        try
                        {
                            var i7 = 10;
                            var i8 = 10;
                            while(true){}
                            do{
                                var i9 = 10;
                            }while(true);
                        }
                        finally{
                            var i13 = 10;
                            var i14 = 10;
                            var i15 = 10;
                        }
                    }
                    else
                    {
                        var i16 = 10;
                    }
                }
            }
        }
    }")]
        public async Task Long_Method(string test)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.LongMethod).WithLocation(0).WithArguments("Run");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}