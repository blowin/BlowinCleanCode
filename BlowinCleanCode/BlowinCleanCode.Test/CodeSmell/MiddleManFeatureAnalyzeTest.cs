using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test.CodeSmell
{
    public class MiddleManFeatureAnalyzeTest
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
        interface IStringFormatOld
        {
            string Format();
        }

        interface IStringFormat
        {
            string Format();
        }

        class {|#0:StringFormat|} : IStringFormat
        {
            private readonly IStringFormatOld _oldFormatter;

            public StringFormat(IStringFormatOld oldFormatter)
                => _oldFormatter = oldFormatter;

            public string Format()
            {
                return _oldFormatter.Format();
            }
        }
    }", "StringFormat", "_oldFormatter")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        interface IStringFormatOld
        {
            string Format();
        }

        interface IStringFormat
        {
            string Format();
        }

        struct {|#0:StringFormat|} : IStringFormat
        {
            private readonly IStringFormatOld _oldFormatter;

            public StringFormat(IStringFormatOld oldFormatter)
                => _oldFormatter = oldFormatter;

            public string Format()
            {
                return _oldFormatter.Format();
            }
        }
    }", "StringFormat", "_oldFormatter")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        interface IStringFormatOld
        {
            string Format();
        }

        interface IStringFormat
        {
            string Format();
        }

        class {|#0:StringFormat|} : IStringFormat
        {
            private readonly IStringFormatOld _oldFormatter;

            public StringFormat(IStringFormatOld oldFormatter)
                => _oldFormatter = oldFormatter;

            public string Format() => _oldFormatter.Format();
        }
    }", "StringFormat", "_oldFormatter")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        interface IStringFormatOld
        {
            string Format();
        }

        interface IStringFormat
        {
            string Format();
        }

        struct {|#0:StringFormat|} : IStringFormat
        {
            private readonly IStringFormatOld _oldFormatter;

            public StringFormat(IStringFormatOld oldFormatter)
                => _oldFormatter = oldFormatter;

            public string Format() => _oldFormatter.Format();
        }
    }", "StringFormat", "_oldFormatter")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        interface IStringFormatOld
        {
            string Format();
        }

        interface IStringFormat
        {
            string Format();
        }

        struct {|#0:StringFormat|} : IStringFormat
        {
            private readonly IStringFormatOld _oldFormatter;

            public StringFormat(IStringFormatOld oldFormatter)
                => _oldFormatter = oldFormatter;

            public string Format() => _oldFormatter.Format();
            public string FormatNew() => _oldFormatter.Format();
        }
    }", "StringFormat", "_oldFormatter")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        interface IStringFormat
        {
            string Format(string format);
        }

        struct {|#0:StringFormat|} : IStringFormat
        {
            private readonly IStringFormat _oldFormatter;

            public StringFormat(IStringFormat oldFormatter)
                => _oldFormatter = oldFormatter;

            public string Format(string format) => _oldFormatter.Format(format);
        }
    }", "StringFormat", "_oldFormatter")]
        [InlineData(@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        interface IStringFormat
        {
            string Format(string format);
        }

        struct {|#0:StringFormat|} : IStringFormat
        {
            private readonly IStringFormat _oldFormatter;

            public StringFormat(IStringFormat oldFormatter)
                => _oldFormatter = oldFormatter;

            public string Format(string format) => _oldFormatter.Format(format?.ToUpper());
        }
    }", "StringFormat", "_oldFormatter")]
        public async Task Invalid(string test, string typeName, string variableAdapter)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.MiddleMan)
                .WithLocation(0)
                .WithArguments(typeName, variableAdapter);

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
        class Adapter
        { 
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
        class DictionaryAdapter
        {   
          
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
        struct Adapter
        { 
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
        struct DictionaryAdapter
        {   
          
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
        interface IStringFormat
        {
            string Format();
        }

        class ArrayStringFormat<T> : IStringFormat
        {
            private readonly T[] _items;
            private readonly Func<T, string> _elementMapper;

            public ArrayStringFormat(T[] items, Func<T, string> elementMapper)
            {
                _items = items;
                _elementMapper = elementMapper ?? (arg => arg.ToString()) ;
            }

            public string Format()
            {
                var mapElements = _items.Select(_elementMapper);
                return string.Join("", "", mapElements);
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
        interface IStringFormat
        {
            string Format();
        }

        struct ArrayStringFormat<T> : IStringFormat
        {
            private readonly T[] _items;
            private readonly Func<T, string> _elementMapper;

            public ArrayStringFormat(T[] items, Func<T, string> elementMapper)
            {
                _items = items;
                _elementMapper = elementMapper ?? (arg => arg.ToString()) ;
            }

            public string Format()
            {
                var mapElements = _items.Select(_elementMapper);
                return string.Join("", "", mapElements);
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
        interface IStringFormatOld
        {
            string Format();
        }

        interface IStringFormat
        {
            string Format();
        }

        class StringFormatAdapter : IStringFormat
        {
            private readonly IStringFormatOld _oldFormatter;

            public StringFormatAdapter(IStringFormatOld oldFormatter)
                => _oldFormatter = oldFormatter;

            public string Format()
            {
                return _oldFormatter.Format();
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
        interface IStringFormatOld
        {
            string Format();
        }

        interface IStringFormat
        {
            string Format();
        }

        struct StringFormatAdapter : IStringFormat
        {
            private readonly IStringFormatOld _oldFormatter;

            public StringFormatAdapter(IStringFormatOld oldFormatter)
                => _oldFormatter = oldFormatter;

            public string Format()
            {
                return _oldFormatter.Format();
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
        interface IStringFormatOld
        {
            string Format();
        }

        interface IStringFormat
        {
            string Format();
        }

        class StringFormatAdapter : IStringFormat
        {
            private readonly IStringFormatOld _oldFormatter;

            public StringFormatAdapter(IStringFormatOld oldFormatter)
                => _oldFormatter = oldFormatter;

            public string Format() => _oldFormatter.Format();
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
        interface IStringFormatOld
        {
            string Format();
        }

        interface IStringFormat
        {
            string Format();
        }

        struct StringFormatAdapter : IStringFormat
        {
            private readonly IStringFormatOld _oldFormatter;

            public StringFormatAdapter(IStringFormatOld oldFormatter)
                => _oldFormatter = oldFormatter;

            public string Format() => _oldFormatter.Format();
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
        public sealed class AppContext
        {
            
        }
        
        public sealed class Cache
        {
            public T GetObject<T>(AppContext context, string key, TimeSpan expireTime, Func<T> factory)
                => factory();
        }

        public class Test
        {
            public bool IsValid(Cache cache)
            {
                return cache.GetObject(new AppContext(), key: ""IsValid"", TimeSpan.FromMinutes(10), () => true);
            }

            public bool IsValidNew(Cache cache)
            {
                return cache.GetObject(new AppContext(), key: ""IsValid"", TimeSpan.FromMinutes(10), () => true);
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
        public sealed class AppContext
        {
            
        }
        
        public sealed class Cache
        {
            public T GetObject<T>(AppContext context, string key, TimeSpan expireTime, Func<T> factory)
                => factory();
        }

        public class Test
        {
            public bool IsValid(Cache cache)
            {
                return cache.GetObject(new AppContext(), key: ""IsValid"", TimeSpan.FromMinutes(10), () => true);
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
        interface IStringFormatOld
        {
            string Format();
        }

        interface IStringFormat
        {
            string Format();
        }

        struct StringFormat : IStringFormat
        {
            private readonly IStringFormatOld _oldFormatter;
            private readonly IStringFormatOld _oldFormatter2;

            public StringFormat(IStringFormatOld oldFormatter, IStringFormatOld oldFormatter2)
                => (_oldFormatter, _oldFormatter2) = (oldFormatter, oldFormatter2);

            public string Format() => _oldFormatter.Format();
            
            public string FormatNew() => _oldFormatter2.Format();
        }
    }")]
        public async Task Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}
