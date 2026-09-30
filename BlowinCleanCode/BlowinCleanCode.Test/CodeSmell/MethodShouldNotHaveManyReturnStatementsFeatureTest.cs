using System.Threading.Tasks;
using BlowinCleanCode.Model.Settings;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.CodeSmell;

public class MethodShouldNotHaveManyReturnStatementsFeatureTest
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
            public int Get(int[] array, int idx)
            {
                return array[idx];
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
        class Test
        {
            public int Get(int[] array, int idx) => array[idx];
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
            public int Get(int[] array, int idx)
            {
                if(array[idx] > 0)
                    return 1;
                return -1;
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
        // Disable BCC4002
        class Test
        {
            public bool Get(int[] array, int idx)
            {
                if(array[idx] > 0)
                    return true;

                if(array[idx] < 0)
                    return true;

                if(array[idx] == 333)
                    return true;

                if(array[idx] == 444)
                    return true;

                return false;
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
        // Disable BCC2000
        class Test
        {
            public int Get(int[] array, int idx)
            {
                Run(() => { return; });
                Run(() => { return; });
                Run(() => { return; });
                Run(() => { return; });
                Run(() => { return; });
                Run(() => { return; });
                Run(() => { return; });
                Run(() => { return; });
                Run(() => { return; });
                Run(() => { return; });
                Run(() => { return; });
                Run(() => { return; });
                return 1;
            }

            private void Run(Action a) {}
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
        // Disable BCC4002
        class Test
        {
            public int Get(int[] array, int idx)
            {
                if(array[idx] > 0)
                {
                    return 1;
                }
                else
                {
                    switch(array[idx])
                    {
                        case 333:
                            return 333;
                        case 444:
                            return 444;
                        default:
                            return 21;
                    }
                }

                return -1;
            }
        }
    }")]
    public async Task Valid(string test)
    {
        await VerifyCS.VerifyAnalyzerAsync(test);
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
        // Disable BCC4002
        class Test
        {
            public int {|#0:Get|}(int[] array, int idx)
            {
                if(array[idx] > 0)
                    return 1;

                if(array[idx] < 0)
                    return 21;

                if(array[idx] == 333)
                    return 333;

                if(array[idx] == 444)
                    return 444;

                return -1;
            }
        }
    }", 5)]
    [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        // Disable BCC4002
        class Test
        {
            public int {|#0:Get|}(int[] array, int idx)
            {
                if(array[idx] > 0)
                    return 1;

                if(array[idx] < 0)
                {
                    if(array[idx] == 333)
                    {
                        if(array[idx] == 444)
                            return 444;
                        
                        return 333;
                    }   
                    
                    return 21;
                }

                return -1;
            }
        }
    }", 5)]
    [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        // Disable BCC4002
        class Test
        {
            public int {|#0:Get|}(int[] array, int idx)
            {
                if(array[idx] > 0)
                    return 1;

                while(array[idx] < 0)
                {
                    if(array[idx] == 333)
                    {
                        if(array[idx] == 444)
                            return 444;
                        
                        return 333;
                    }   
                    
                    return 21;
                }

                return -1;
            }
        }
    }", 5)]
    [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        // Disable BCC4002
        class Test
        {
            public int {|#0:Get|}(int[] array, int idx)
            {
                if(array[idx] > 0)
                    return 1;

                {
                    if(array[idx] == 333)
                    {
                        if(array[idx] == 444)
                            return 444;
                        
                        return 333;
                    }   
                    
                    return 21;
                }

                return -1;
            }
        }
    }", 5)]
    public async Task Invalid(string test, int actualCountReturnStatement)
    {
        var expected = VerifyCS.Diagnostic(Constant.Id.MethodShouldNotHaveManyReturnStatements)
            .WithLocation(0)
            .WithArguments(actualCountReturnStatement, AnalyzerSettings.Instance.MaxReturnStatement);

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
        // Disable BCC4002
        // Disable BCC2000
        class Test
        {
            public bool {|#0:Get|}(int[] array, int idx)
            {
                if(array[idx] > 0)
                    return true;

                {
                    if(array[idx] == 333)
                    {
                        if(array[idx] == 444)
                            return true;
                        
                        return false;
                    }   
                    
                    return true;
                }

                {
                    return false;
                    return false;
                    return false;
                    return false;
                    return false;
                }

                return false;
            }
        }
    }", 10)]
    public async Task Invalid_BoolReturn(string test, int actualCountReturnStatement)
    {
        var expected = VerifyCS.Diagnostic(Constant.Id.MethodShouldNotHaveManyReturnStatements)
            .WithLocation(0)
            .WithArguments(actualCountReturnStatement, AnalyzerSettings.Instance.MaxReturnStatementForReturnBool);

        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    private const string TwoReturnsBoolMethod = @"
    using System;

    namespace ConsoleApplication1
    {
        class Test
        {
            public bool {|#0:Run|}(int value)
            {
                if (value > 0)
                    return true;

                return false;
            }
        }
    }";

    [Fact]
    public async Task Threshold_Is_Not_Overridden_Without_EditorConfig()
    {
        // Two returns are within the compiled-in limit.
        await VerifyCS.VerifyAnalyzerAsync(TwoReturnsBoolMethod);
    }

    [Fact]
    public async Task Threshold_Is_Overridden_By_EditorConfig()
    {
        var editorConfig = "[*.cs]\n" + Constant.Option.MaxReturnStatementForReturnBool + " = 1";

        var expected = VerifyCS.Diagnostic(Constant.Id.MethodShouldNotHaveManyReturnStatements)
            .WithLocation(0)
            .WithArguments(2, 1);

        await VerifyCS.VerifyAnalyzerAsync(TwoReturnsBoolMethod, editorConfig, expected);
    }
}