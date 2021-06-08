using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    [TestClass]
    public class BlowinCleanCodeUnitTest
    {
        [TestMethod]
        public async Task Empty_Input_Verify_Analyzer()
        {
            var test = @"";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        [TestMethod]
        public async Task Without_Problem_Program_Verify_Analyzer()
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
        class Calculator
        {   
            public int {|#0:Sum|}(int v1, int v2) => v1 + v2;
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
        
        [TestMethod]
        public async Task Public_Static_Field()
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
        class Test
        {   
            public static int {|#0:Value|};
        }
    }";
            var expected = VerifyCS.Diagnostic(Constant.Id.PublicStaticField).WithLocation(0).WithArguments("Value");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [TestMethod]
        public async Task Long_Method()
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
            }
        }
    }";
            var expected = VerifyCS.Diagnostic(Constant.Id.LongMethod).WithLocation(0).WithArguments("Run");
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

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

        [TestMethod]
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
        class TEST
        {   
            // Disable BCC2002
            public static void {|#0:RunAndClose|}()
            {

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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
            public float Calculate(int quantity, float price)
            {
                return quantity * price * {|#0:1.2f|};
            }
        }
    }", "1.2f")]
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
            public float Calculate(int quantity, float price) => quantity * price * {|#0:1.2f|};
        }
    }", "1.2f")]
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
            public void Run(int count)
            {
                Sum(count, {|#0:10|});
            }

            public int Sum(int v1, int v2) => v1 + v2;
        }
    }", "10")]
        public async Task Method_Contain_Magic_Value(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.MagicValue).WithLocation(0).WithArguments(argument);
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
        class Test
        {
            public float Calculate(int quantity, float price)
            {
                float value = 1.2f;
                return quantity * price * value;
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
            public float Calculate(int quantity, float price)
            {
                const float value = 1.2f;
                return quantity * price * value;
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
            public void Run(int count)
            {
                for(var i = 0; i < 10; i += 1){
                
                }
            }
        }
    }")]
        public async Task Method_Contain_Magic_Value_As_Variable(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
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
        class Test
        {
            public string Calculate(int age)
            {
                if(age > 18)
                    return ""Oh my)"";

                return {|#0:null|};
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
            public string Calculate()
            {
                return {|#0:null|};
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
            public string Calculate() => {|#0:null|};
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
            public string Calculate(int age)
            {
                if(age > 18)
                    return {|#0:null|};

                return ""Oh my)"";
            }
        }
    }")]
        public async Task Method_Return_Null(string test)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.ReturnNull).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}
