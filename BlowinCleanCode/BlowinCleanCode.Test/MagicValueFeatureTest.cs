using System;
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
                for(var i = 0; i < count; i += 1){
                
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
            public void Run(string name) {
                Console.WriteLine(name);
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
            public void Run(string name) {
                WriteLine(name);
            }

            private void WriteLine(string str) {}
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
            public void Run(string name) {
                var msg = ""test msg "" + name;
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
            public static (int Age, bool Flag) Run(int age, bool f)
            {
                if(age > 0)
                    return (age, true);
                return default;
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
            public static (int Age, bool Flag) Run(int age, bool f)
            {
                if(age > 0)
                    return (age, true);
                return age > 0 ? default : (age, f);
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
            // Disable BCC4000
            public static (int Age, bool Flag) Run(int age, bool f)
            {
                if(age > 0)
                    return (age, true);
                return age > 0 ? (f != f ? default : (age, f)) : (age + 1, f);
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
            enum MyEnum
            {
                Case1 = 1,
                Case2 = 2,
                Case3 = 3
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
            public void Run(string name) {
                name = ""newValue"";
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
            public void Run(string name) {
                Run2(""newValue"");
            }

            public void Run2(string name) {
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
        static class Test
        {
            public static void Run(this object self, string name) {
                self.Run(""newValue"");
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
            public bool Check(int number)
            {
                return number.Equals({|#0:3|});
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
            // Disable BCC4000
            public void Run(int age, bool f)
            {
                var border = 18;
                var res = age >= border ? true : false;
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
            // Disable BCC4000
            public void Run(int age, bool f)
            {
                var border = 18;
                bool res = false;
                res = age >= border ? true : false;
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
            // Disable BCC4000
            public void Run(int age, bool f)
            {
                var border = 18;
                var obj = new Person { Flag = age >= border ? true : false };
            }
        }

        class Person {
            public bool Flag { get; set; }
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
            public void Run(int age, bool f)
            {
                
            }

            public void Run2(int num)
            {
                if (num > 0)
                {
                    Run(age: 18, f: true);   
                }
                else
                {
                    Run(age: 22, f: false);
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
            public void Run(int age, bool f)
            {
                
            }

            public void Run2(int num)
            {
                Run(age: 18, f: true);
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
            public bool Run(int age, bool f) => true;
        }

        class Test2
        {
            public void Run(Test test)
            {
                var result = test.Run(age: 18, f: true);
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
            public void Run(int age, bool f = false, bool f2 = false)
            {
                
            }

            public void Run2(int num)
            {
                Run(18);
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
            private enum MyType
            {
                Case1,
                Case2,
            }
            
            private string Run(MyType type, int v)
            {
                switch (type)
                {
                    case MyType.Case1:
                        return v == 0 ? ""V1"" : ""V2"";
                    case MyType.Case2:
                        return ""V3"";
                    default:
                        return ""V4"";
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
        public class MyAttribute : Attribute
        {
            public string Name { get; set; }
        }
        
        public class MyClass
        {
            [My(Name = ""Test"")]
            public string Name { get; set; }
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
        public class MyAttribute : Attribute
        {
            public string Name { get; }

            public MyAttribute(string name) => Name = name;
        }
        
        public class MyClass
        {
            [My(""Test"")]
            public string Name { get; set; }
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
        public class MyAttribute : Attribute
        {
            public string Name { get; set; }
        }
        
        public class MyClass
        {
            [My(Name = ""Test"")]
            public void Name(){}
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
        public class MyAttribute : Attribute
        {
            public string Name { get; }

            public MyAttribute(string name) => Name = name;
        }
        
        public class MyClass
        {
            [My(""Test"")]
            public void Name(){}
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
            public void Run(string name) {
                Run2({|#0:""newValue""|}, name);
            }

            public void Run2(string name, string name2) {
            }
        }
    }", "\"newValue\"")]
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
            // Disable BCC4000
            public void Run(int age, bool f)
            {
                var border = 18;
                var res = age >= border ? Calculate({|#0:true|}, f, f) : false;
            }

            public bool Calculate(bool v, bool v2, bool v3) => v;
        }
    }", "true")]
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
            // Disable BCC4000
            public void Run(int age, bool f)
            {
                var border = 18;
                bool res = false;
                res = age >= border ? Calculate({|#0:true|}, f, f) : false;
            }

            public bool Calculate(bool v, bool v2, bool v3) => v;
        }
    }", "true")]
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
            // Disable BCC4000
            public void Run(int age, bool f)
            {
                var border = 18;
                var obj = new Person { Flag = age >= border ? Calculate({|#0:true|}, f, f) : false };
            }

            public bool Calculate(bool v, bool v2, bool v3) => v;
        }

        class Person {
            public bool Flag { get; set; }
        }
    }", "true")]
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
            // Disable BCC4000
            public void Run(int age, bool f)
            {
                var border = 18;
                var obj = new Person();
                obj.Flag = age >= border ? Calculate({|#0:true|}, f, f) : false;
            }

            public bool Calculate(bool v, bool v2, bool v3) => v;
        }

        class Person {
            public bool Flag { get; set; }
        }
    }", "true")]
        public async Task Invalid(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.MagicValue).WithLocation(0).WithArguments(argument);
            await VerifyCS.VerifyAnalyzerAsync(test, expected);
        }
    }
}