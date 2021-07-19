using System.Threading.Tasks;
using Xunit;
using VerifyCS = BlowinCleanCode.Test.Verifiers.CSharpAnalyzerVerifier<BlowinCleanCode.BlowinCleanCodeAnalyzer>;

namespace BlowinCleanCode.Test
{
    public class DeeplyNestedCodeFeatureAnalyzeTest
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
    // Disable BCC2000
     public static void Run(int age)
     {
         {|#0:if(age > 19){       
             while(age > 0)
             {
                 age -= 1;
                 if(age <= 0)
                 {
                     if(age == 0)
                     {
                         break;
                     }
                 }
             }
         }|}
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
     // Disable BCC2000
     public static void Run(int age)
     {
         {|#0:if(age > 19){
             if(age != 100){
             }

             while(age > 0)
             {
                 age -= 1;
                 if(age <= 0)
                 {
                     if(age == 0)
                     {
                         break;
                     }
                 }
             }
         }|}
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
     // Disable BCC2000
     public static void Run(int age)
     {
         if(age > 19){
             if(age != 100){
             }

             while(age > 0)
             {
             }
         }

         {|#0:if(age > 19){
             if(age != 100){
             }

             while(age > 0)
             {
                 age -= 1;
                 if(age <= 0)
                 {
                     if(age == 0)
                     {
                         break;
                     }
                 }
             }
         }|}
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
     // Disable BCC2000
     public static void Run(int age, int limit)
     {
         if(age > limit){
             
         }
         else {|#0:if(age > 19){
             if(age != 100){
             }

             while(age > 0)
             {
                 age -= 1;
                 if(age <= 0)
                 {
                     if(age == 0)
                     {
                         break;
                     }
                 }
             }
         }|}
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
     // Disable BCC2000
     public static void Run(int age, IDisposable d)
     {
         {|#0:if(age > 19){       
             while(age > 0)
             {
                 age -= 1;
                 if(age <= 0)
                 {
                     using(d)
                    {
                        break;
                    }
                 }
             }
         }|}
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
                // Disable BCC2000
                public static void Run(int age, int limit)
                {
                    {|#0:if(age > limit){
                        if(age != age){
                        }
                        else if(age > 19){
                            if(age != 100){
                            }
           
                            while(age > 0)
                            {
                                age -= 1;
                                if(age <= 0)
                                {
                                    if(age == 0)
                                        break;
                                }
                            }
                        }
                    }|}
                }
            }
        }")]
        public async Task Invalid(string test)
        {
            var expected = VerifyCS.Diagnostic(Constant.Id.DeeplyNestedCode).WithLocation(0);
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
            public static int Run(List<int> seq)
            {
                var sum = seq
                .OrderBy(e => e)
                .Select(e =>
                {
                    var str = e.ToString();
                    var strLen = str.Length;
                    var curValue = e;
                    return strLen * curValue;
                })
                .AsParallel()
                .AsUnordered()
                .Sum();
            
                var sum2 = seq
                    .OrderBy(e => e)
                    .Select(e => e.ToString().Length)
                    .AsParallel()
                    .AsUnordered()
                    .Sum();

                var result = sum + sum2;
                return result;
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
            public static void Run(int age)
            {
                if(age > 19){
                    if(age != 100){
                    }
   
                    if(age != 100){
                    }
                    
                    if(age != 100){
                    }
                    while(age > 0)
                    {
                        
                    }
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
            // Disable BCC2000
            public static void Run(int age)
            {
                if(age > 19){
                    if(age != 100){
                    }
   
                    while(age > 0)
                    {
                    }
                }

                if(age > 19){
                    if(age != 100){
                    }
   
                    while(age > 0)
                    {
                        age -= 1;
                        if(age <= 0)
                        {
                        }
                    }
                }

                while(age > 0)
                {
                    age -= 1;
                    if(age <= 0)
                    {
                    }
                }

                while(age > 0)
                {
                    age -= 1;
                    if(age <= 0)
                    {
                    }
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
            public static void Run(int age, int limit)
            {
                if(age > limit){
                    
                }
                else if(age == limit){
                
                }
                else if(age == limit){
                
                }
                else if(age == limit){
                
                }
                else if(age == limit){
                
                }
                else if(age == limit){
                
                } 
            }
        }
    }")]
        public async Task Valid(string test)
        {
            await VerifyCS.VerifyAnalyzerAsync(test);
        }
    }
}