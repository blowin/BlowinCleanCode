using System.Threading.Tasks;
using BlowinCleanCode.Model;
using BlowinCleanCode.Model.Settings;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.SingleResponsibility
{
    public class MethodALotOfDeclarationFeatureTest
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
            }
        }
    }", "Run", 11)]
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
            // Disable BCC2000
            // Disable BCC4002
            public void {|#0:Run|}(int n)
            {
                if(true)
                {
                    var i1 = 10;
                    for(var j = 0; j < 0; j++)
                    {
                        var i2 = 10;
                        var i3 = 10;
                        var i4 = 10;
                    }
                }
                var i5 = 10;
                var i6 = 10;
                var i7 = 10;
                var i8 = 10;
                var i9 = 10;
                var i10 = 10;
                var i11 = 10;
            }
        }
    }", "Run", 11)]
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
            // Disable BCC2000
            // Disable BCC4002
            public void {|#0:Run|}(IEnumerable<int> items)
            {
                if(true)
                {
                    var i1 = 10;
                    for(var j = 0; j < 0; j++)
                    {
                        var i2 = 10;
                        var i3 = 10;
                        var i4 = 10;
                    }
                }
                var i5 = 10;
                var i6 = 10;
                var i7 = 10;
                var i8 = 10;
                var i9 = 10;
                var i10 = 10;
                const int maxNumber = 0;
                var i11 = items.Where(i => i > maxNumber);
            }
        }
    }", "Run", 11)]
        public async Task Method_A_Lot_Of_Declaration(string test, string argument, int actual)
        {
            var settings = new AnalyzerSettings();
            var expected = VerifyCS.Diagnostic(Constant.Id.MethodContainALotOfDeclaration).WithLocation(0).WithArguments(argument, actual, settings.MaxMethodDeclaration);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}