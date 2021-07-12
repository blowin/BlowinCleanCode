using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    [TestClass]
    public class PreserveWholeObjectFeatureTest
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
        class TEST
        {   
            public void Run(Data data) => {|#0:Handle(data.Age, data.Sex, data.Child.Country.Name, data.Child.Country.Code)|};

            private void Handle(int age, bool sex, string name, string code) {} 

            public sealed class Data
            {
                public int Age { get; }
                public bool Sex { get; }
                public int FirstName { get; }
                public int LastName { get; }
                public Country Country { get; }

                public Data Child { get; }
            }

            public sealed class Country
            {
                public string Name { get; }
                public string Code { get; }
            }
        }
    }", @"data")]
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
            public void Run(Data data) => {|#0:Handle(data.Age, data.Sex, data.Country.Name, data.Country.Code)|};

            private void Handle(int age, bool sex, string name, string code) {} 

            public sealed class Data
            {
                public int Age { get; }
                public bool Sex { get; }
                public int FirstName { get; }
                public int LastName { get; }
                public Country Country { get; }
            }

            public sealed class Country
            {
                public string Name { get; }
                public string Code { get; }
            }
        }
    }", @"data")]
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
            public void Run(Data data, Data data2) => {|#0:Handle(data.Age, data.Sex, data2.Country.Name, data2.Country.Code)|};

            private void Handle(int age, bool sex, string name, string code) {} 

            public sealed class Data
            {
                public int Age { get; }
                public bool Sex { get; }
                public int FirstName { get; }
                public int LastName { get; }
                public Country Country { get; }
            }

            public sealed class Country
            {
                public string Name { get; }
                public string Code { get; }
            }
        }
    }", @"data and data2")]
        public async Task Invalid(string test, string argument)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.PreserveWholeObject).WithLocation(0).WithArguments(argument);
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
            public void Run(Data data) => Handle(data.Age);

            private void Handle(int age) {} 

            public sealed class Data
            {
                public int Age { get; }
                public bool Sex { get; }
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
        class TEST
        {   
            public void Run(Data data) => {|#0:Handle(data.Age)|};

            private void Handle(int age) {} 

            public sealed class Data
            {
                public int Age { get; }
            }
        }
    }")]
        public async Task Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}