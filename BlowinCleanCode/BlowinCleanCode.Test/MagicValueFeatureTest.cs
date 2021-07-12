using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    public class MagicValueFeatureTest
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
            public string BuildMessage()
            {
                return new StringBuilder(256).Append(""Hello "").Append(""world!"").ToString();
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
            public string HelloMessage()
            {
                return string.Concat(""Hello"", "" "", ""World!"");
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
            public string[] Split(string val)
            {
                return val.Split(' ');
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
            public string Format(DateTime date)
            {
                return date.ToString(""dd-MM-yyyy"");
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
            public bool IsValid(int[] array, int idx)
            {
                return array[idx] > 0 ? true : false;
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
            public float Calculate(int quantity, float price)
            {
                float value = 1.2f;
                return quantity * price * value;
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
            public float Calculate(int quantity, float price)
            {
                const float value = 1.2f;
                return quantity * price * value;
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
            public void Run(int count)
            {
                for(var i = 0; i < 10; i += 1){
                
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
        class Test
        {
            public void Run(int count)
            {
                Sum(count, v2: 10);
            }

            public int Sum(int v1, int v2) => v1 + v2;
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
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        // Disable BCC2003
        class Test
        {
            public string DisplayName(bool f) {
                var res = f ? ""test1"" : ""test2"";
                return res;
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
            public bool IsAdmin(string name) {
                const string adminName = ""admin"";
                if(name == adminName)
                    return true;

                return false;
            }
        }
    }")]
        public async Task Method_Contain_Magic_Value_Valid(string test)
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
            public float Calculate(int quantity, float price)
            {
                return quantity * price * {|#0:1.2f|};
            }
        }
    }", "1.2f")]
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
            public float Calculate(int quantity, float price) => quantity * price * {|#0:1.2f|};
        }
    }", "1.2f")]
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
            public void Run(int count)
            {
                Sum(count, {|#0:10|});
            }

            public int Sum(int v1, int v2) => v1 + v2;
        }
    }", "10")]
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
            public bool Check(int number)
            {
                return number.Equals({|#0:3|});
            }
        }
    }", "3")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class Person{
            public string Name{get;}
            public int Age{get;}
            private Person(string name, int age)
            {
                Name = name;
                Age = age;
            }

            public static Person Create(string name, int age) => new Person(name, age);

            public static Person Test(int age) => Create({|#0:""Test""|}, age);
        }
    }", "\"Test\"")]
        public async Task Method_Contain_Magic_Value(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.MagicValue).WithLocation(0).WithArguments(argument);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}