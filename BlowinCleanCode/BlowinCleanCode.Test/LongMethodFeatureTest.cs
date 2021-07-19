using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    public class LongMethodFeatureTest
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
        // Disable BCC2004
        // Disable BCC4002
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
                var i17 = 10;
                var i18 = 10;
                var i19 = 10;
                var i20 = 10;
                var i21 = 10;
                var i22 = 10;
                var i23 = 10;
                var i24 = 10;
                var i25 = 10;
                var i26 = 10;
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
        // Disable BCC2004
        // Disable BCC4002
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
                    var i16 = 10;
                    var i17 = 10;
                    var i18 = 10;
                    var i19 = 10;
                    var i20 = 10;
                    var i21 = 10;
                    var i22 = 10;
                    var i23 = 10;
                    var i24 = 10;
                    var i25 = 10;
                    var i26 = 10;
                }
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
        // Disable BCC2004
        // Disable BCC4002
        // Disable BCC4005
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
                        var i17 = 10;
                        var i18 = 10;
                        var i19 = 10;
                        var i20 = 10;
                    }
                }
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
        // Disable BCC2004
        // Disable BCC4002
        class Test
        {   
            public static void {|#0:Run|}(List<int> seq)
            {
                seq.Select(e => {
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
                    var i17 = 10;
                    var i18 = 10;
                    var i19 = 10;
                    var i20 = 10;
                    var i21 = 10;
                    var i22 = 10;
                    var i23 = 10;
                    var i24 = 10;
                    var i25 = 10;
                    var i26 = 10;
                    return e;
                });
            }
        }
    }")]
        public async Task Invalid(string test)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.LongMethod).WithLocation(0).WithArguments("Run");
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
        class Test
        {
            public static int Run(List<int> seq)
            {
                var sum = seq
                .OrderBy(e => e)
                .Select(e =>
                {
                    var str = e.ToString();
                    var strLen = str.Length;
                    var curValue = e;
                    return strLen * curValue;
                })
                .AsParallel()
                .AsUnordered()
                .Sum();
            
                var sum2 = seq
                    .OrderBy(e => e)
                    .Select(e => e.ToString().Length)
                    .AsParallel()
                    .AsUnordered()
                    .Sum();

                var result = sum + sum2;
                return result;
            }
        }
    }")]
        public async Task Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}