using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.GoodPractice
{
    public class ThreadStaticFieldsShouldNotBeInitializedFeatureAnalyzeTest
    {
        [Theory]
        [InlineData(@"public class Test
        {
            [System.ThreadStatic]
            private static int {|#0:TestField1|} = 1;
        }", "TestField1")]
        [InlineData(@"public class Test
        {
            [System.ThreadStatic]
            private static int {|#0:TestField2|} = CreateValue();

            private static int CreateValue() => 1;
        }", "TestField2")]
        public async Task Invalid(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.ThreadStaticFieldsShouldNotBeInitialized).WithArguments(argument).WithLocation(0);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }

        [Theory]
        [InlineData(@"public class Test
        {
            [System.ThreadStatic]
            private static int TestField;
        }")]
        [InlineData(@"public class Test
        {
            private static readonly int TestField = 2;
        }")]
        public async Task Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
