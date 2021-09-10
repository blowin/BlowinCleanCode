using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.GoodPractice
{
    public class TypeThatProvideEqualsShouldImplementIEquatableFeatureTest
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
        class {|#0:Person|}
        {
            public int Age { get; set; }

            public bool Equals(Person other) => Age == other.Age;

            public override bool Equals(object obj) => obj is Person other && Equals(other);

            public override int GetHashCode() => Age;
        }
    }", "Person")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class {|#0:Person|}
        {
            public int Age { get; set; }

            public bool Equals(int other) => Age == other;

            public override bool Equals(object obj) => obj is Person other && Equals(other);

            public override int GetHashCode() => Age;
        }
    }", "Int32")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        struct {|#0:Person|}
        {
            public int Age { get; set; }

            public bool Equals(Person other) => Age == other.Age;

            public override bool Equals(object obj) => obj is Person other && Equals(other);

            public override int GetHashCode() => Age;
        }
    }", "Person")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        struct {|#0:Person|}
        {
            public int Age { get; set; }

            public bool Equals(int other) => Age == other;

            public override bool Equals(object obj) => obj is Person other && Equals(other);

            public override int GetHashCode() => Age;
        }
    }", "Int32")]
        public async Task Invalid(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.TypeThatProvideEqualsShouldImplementIEquatable).WithArguments(argument).WithLocation(0);
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
        public class Person : IEquatable<Person>
        {
            public int Age { get; set; }

            public bool Equals(Person other) => Age == other.Age;

            public override bool Equals(object obj) => obj is Person other && Equals(other);

            public override int GetHashCode() => Age;
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
        public class Person : IEquatable<Person>
        {
            public int Age { get; set; }

            public bool Equals(Person other)
            {
                return Age == other.Age;
            }

            public override bool Equals(object obj) => obj is Person other && Equals(other);

            public override int GetHashCode() => Age;
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
        public struct Person : IEquatable<Person>
        {
            public int Age { get; set; }

            public bool Equals(Person other) => Age == other.Age;

            public override bool Equals(object obj) => obj is Person other && Equals(other);

            public override int GetHashCode() => Age;
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
        public struct Person : IEquatable<Person>
        {
            public int Age { get; set; }

            public bool Equals(Person other)
            {
                return Age == other.Age;
            }

            public override bool Equals(object obj) => obj is Person other && Equals(other);

            public override int GetHashCode() => Age;
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
        public class Wrapper
        {
            public struct Person : IEquatable<Person>
            {
                public int Age { get; set; }

                public bool Equals(Person other)
                {
                    return Age == other.Age;
                }

                public override bool Equals(object obj) => obj is Person other && Equals(other);

                public override int GetHashCode() => Age;
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
        public class Wrapper
        {
            public struct Person : IEquatable<Person>
            {
                public int Age { get; set; }

                public bool Equals(Person other) => Age == other.Age;

                public override bool Equals(object obj) => obj is Person other && Equals(other);

                public override int GetHashCode() => Age;
            }
        }
    }")]
        public async Task Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}