﻿using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.SingleResponsibility
{
    public class ManyMethodParameterFeatureTest
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
            public static void {|#0:Run|}(int i, int i2, int i3, int i4, int i5)
            {

            }
        }
    }", "Run")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        static class Test
        {   
            public static void {|#0:Run|}(this int i, int i2, int i3, int i4, int i5, int i6)
            {

            }
        }
    }", "Run")]
        public async Task Many_Method_Parameters(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.ManyParametersMethod).WithLocation(0).WithArguments(argument);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [Fact]
        public async Task Many_Method_Parameters_Extension()
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
        static class Test
        {   
            public static void Run(this int i, int i2, int i3, int i4, int i5)
            {

            }
        }
    }";

            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}