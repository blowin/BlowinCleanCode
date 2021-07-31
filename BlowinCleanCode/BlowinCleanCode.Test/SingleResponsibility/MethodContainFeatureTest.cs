using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.SingleResponsibility
{
    public class MethodContainFeatureTest
    {
        [Fact]
        public async Task Method_Contain_And()
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
            public static void {|#0:RunAndClose|}()
            {

            }
        }
    }";
            var expected = VerifyCS.Diagnostic(Constant.Id.MethodContainAnd).WithLocation(0).WithArguments("RunAndClose");
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
        class TEST
        {   
            // Disable BCC2002
            public static void {|#0:RunAndClose|}()
            {

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
        // Disable BCC2002
        class TEST
        {   
            public static void {|#0:RunAndClose|}()
            {

            }
        }
    }")]
        public async Task Method_Contain_And_Disable_With_Comment(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task Two_Method_Contain_And_Disable_First_With_Comment()
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
            // Disable BCC2002
            public static void RunAndClose()
            {

            }

            public static void {|#0:RunAndClose2|}()
            {

            }
        }
    }";

            var expected = VerifyCS.Diagnostic(Constant.Id.MethodContainAnd).WithLocation(0).WithArguments("RunAndClose2");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task Method_Contain_Android_Not_Found_And_Diagnostic()
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
            public static void {|#0:RunAndroid|}()
            {

            }
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task Method_Contain_Android_And_And()
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
            public static void {|#0:RunAndroidAndClose|}()
            {

            }
        }
    }";

            var expected = VerifyCS.Diagnostic(Constant.Id.MethodContainAnd).WithLocation(0).WithArguments("RunAndroidAndClose");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task Method_Contain_And_At_End_Of_Name()
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
            public static void {|#0:RunAnd|}()
            {

            }
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}