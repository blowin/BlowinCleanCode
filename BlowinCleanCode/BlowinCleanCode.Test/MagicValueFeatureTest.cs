﻿using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    [TestClass]
    public class MagicValueFeatureTest
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
            public int Get(int[] array, int idx)
            {
                return array[idx];
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
                Sum(count, v2: 10);
            }

            public int Sum(int v1, int v2) => v1 + v2;
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
        public class Person
        {
            public int Age { get; set; }
            public string Name;
        }

        public class Test
        {
            public void Run(int count)
            {
                var p = new Person
                {
                    Age = 19,
                    Name = ""User""
                };
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
            public string DisplayName(bool f) {
                var res = f ? ""test1"" : ""test2"";
                return res;
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
            public void Run(float price)
            {
                var v1 = '1';
                var s2 = ""dima"";
                var s3 = 3;
                var s4 = 3f;
                var s5 = 3.0;
                var s6 = false;
                var s7 = new int[] {1, 2, 3};
                var result = Calculate({|#0:10|}, price);
            }

            public float Calculate(int quantity, float price)
            {
                return quantity * price;
            }
        }
    }", "10")]
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
            public void Run(float price)
            {
                var v1 = '1';
                var s2 = ""dima"";
                var s3 = 3;
                var s4 = 3f;
                var s5 = 3.0;
                var s6 = false;
                var s7 = new int[] {1, 2, 3};
                var result = Calculate({|#0:10|}, price);
            }

            public float Calculate(int quantity, float price)
            {
                return quantity * price;
            }
        }
    }", "10")]
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
    }
}