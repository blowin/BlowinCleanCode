using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.CodeSmell
{
    public class CatchShouldDoMoreThanRethrowFeatureTest
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
            public void Run()
            {
                try
                {
                    Run2();
                }
                catch (Exception e)
                {
                    {|#0:throw;|}
                }       
            }

            private void Run2(){}
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
        class Test
        {
            public void Run()
            {
                try
                {
                    Run2();
                }
                catch (Exception e)
                {
                    {|#0:throw e;|}
                }       
            }

            private void Run2(){}
        }
    }")]
        public async Task Invalid(string test)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.CatchShouldDoMoreThanRethrow).WithLocation(0);
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
            public void Run()
            {
                var exception = new Exception();
                try
                {
                    Run2();
                }
                catch (ArgumentNullException)
                {
                    {|#0:throw;|}
                }
                catch (Exception e)
                {
                    {|#1:throw;|}
                }       
            }

            private void Run2(){}
        }
    }")]
        public async Task Multiple_Catch_Invalid(string test)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.CatchShouldDoMoreThanRethrow)
                .WithLocation(0)
                .WithLocation(1);

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
            public void Run()
            {
                try
                {
                    Run2();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }       
            }

            private void Run2(){}
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
        class Test
        {
            public void Run()
            {
                try
                {
                    Run2();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw e;
                }       
            }

            private void Run2(){}
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
        class Test
        {
            public void Run()
            {
                try
                {
                    Run2();
                }
                catch (Exception e)
                {
                    Run2();
                }       
            }

            private void Run2(){}
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
        class Test
        {
            public void Run()
            {
                try
                {
                    Run2();
                }
                catch (Exception e)
                {
                }       
            }

            private void Run2(){}
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
        class Test
        {
            public void Run()
            {
                try
                {
                    Run2();
                }
                catch (Exception)
                {
                    throw new Exception();
                }       
            }

            private void Run2(){}
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
        class Test
        {
            public void Run()
            {
                var exception = new Exception();
                try
                {
                    Run2();
                }
                catch (Exception)
                {
                    throw exception;
                }       
            }

            private void Run2(){}
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
        class Test
        {
            public void Run()
            {
                var exception = new Exception();
                try
                {
                    Run2();
                }
                catch (ArgumentNullException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }       
            }

            private void Run2(){}
        }
    }")]
        public async Task Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
