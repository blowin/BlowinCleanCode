using System.Linq;
using BlowinCleanCode.Extension;
using BlowinCleanCode.Feature.SingleResponsibility;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace BlowinCleanCode.Test.SingleResponsibility;

public class ComplexityWalkerTest
{
    [Theory]
    [InlineData(@"using System;
using System.Threading.Tasks;

public class A
{
    public void Run(bool b)
    {
        if (b) // +1
            Console.WriteLine();
        else // +1
            Console.WriteLine();
    }
}
", 2)]
    [InlineData(@"using System;
using System.Threading.Tasks;

public class A
{
    public void Run(bool b)
    {
        if (true) // +1
        {
            if (b) // +2 (N=1)
                Console.WriteLine();
            else if (!b) // +2 (N=1)
                Console.WriteLine();
            else // +2 (N=1)
                Console.WriteLine();
        }
    }
}
", 7)]
    [InlineData(@"using System;
using System.Threading.Tasks;

public class A
{
    public void Run(bool b)
    {
        var task = Task.Run(() => // +1
        {
            if (b) // +2 (N=1)
                Console.WriteLine();
        });
    }
}

", 3)]
    [InlineData(@"using System;

public class A
{
    public void Run(bool a, bool b, bool c, bool d)
    {
        var x = a || b || c; // +1
        var x1 = a && !b && c && d; // +1
        var x2 = !(a && b && c); // +1
    }
}
", 3)]
    [InlineData(@"using System;

public class A
{
    public void Run(bool a, bool b, bool c, bool d)
    {
        //        ↓    ↓ +2
        var x = a || b && c;
    }
}
", 2)]
    [InlineData(@"using System;

public class A
{
    public void Run(bool a, bool b, bool c, bool d)
    {
        //         ↓    ↓     +2
        var x = a || b && c && d;
    }
}
", 2)]
    [InlineData(@"using System;

public class A
{
    public void Run(bool a, bool b, bool c, bool d)
    {
        //         ↓    ↓         ↓ +3
        var x = a || b && c && d || a;
    }
}
", 3)]
    [InlineData(@"using System;

public class A
{
    public void Run(bool a, bool b, bool c, bool d)
    {
        //         ↓    ↓         ↓ +3
        var x = a || b && c && d || a || a;
    }
}
", 3)]
    [InlineData(@"using System;

public class A
{
    public void Run(bool a, bool b, bool c, bool d)
    {
        //         ↓    ↓         ↓   ↓  +4
        var x = a || b && c && d || a && a;
    }
}
", 4)]
    [InlineData(@"using System;

public class A
{
    public void Run(bool a, bool b, bool c, bool d, bool e, bool f)
    {
        if (a // +1 for if
            && b && c // +1
            || d || e // +1
            && f) // +1
        {
        }
    }
}
", 4)]
    [InlineData(@"using System;
using System.Reflection.Emit;

public class A
{
    public void Run()
    {
    MyLabel:
        for (var i = 0; i < 100; i++) // +1
        {
            foreach (var c in "") // +2 (N=1)
            {
                while (true) // +3 (N=2)
                {
                    do // +4 (N=3)
                    {
                        if (true) // +5 (N=4)
                            goto MyLabel; // +1
                    } while (false);
                }
            }
        }
    }
}
", 16)]
    [InlineData(@"using System;
using System.Reflection.Emit;

public class A
{
    public void Run()
    {
        for (var i = 0; i < 100; i++) // +1
        {
        }
        
        foreach (var c in "") // +1
        {
        }

        while (true) // +1
        {
        }
        
        do // +1
        {
        } while (false);
        
    MyLabel: // +1
        goto MyLabel;
    }
}
", 5)]
    [InlineData(@"using System;
using System.Reflection.Emit;

public class A
{
    public void Run()
    {
        foreach (var c in "") // +1
        {
            if (true) // +2 (N=1)
                continue; // +1

            if (false) // +2 (N=1)
                break; // +1

            Console.WriteLine();
        }
    }
}
", 7)]
    [InlineData(@"using System;

public class A
{
    public void Run(object obj)
    {
        string str = null;
        if (obj != null) // +1
            str = obj.ToString();
    }
}
", 1)]
    [InlineData(@"using System;

public class A
{
    public void Run(object obj)
    {
        var str = obj?.ToString();
    }
}
", 0)]
    [InlineData(@"using System;

public class A
{
    public void Run()
    {
        Run(); // +1
    }
}
", 1)]
    [InlineData(@"using System;

public class A
{
    public bool Run(bool a)
    {
        if (a && Run(a)) // +3 (if, and, recursive)
        {
            return true;
        }
        
        return false;
    }
}
", 3)]
    [InlineData(@"using System;

public class A
{
    public bool Run(bool a)
    {
        return a && !Run(a); // +2
    }
}
", 2)]
    [InlineData(@"using System;

public class A
{
    public string Run(int number)
    {
        switch (number) // +1
        {
            case 1:
                return ""one"";
            case 2:                   
                return ""a couple"";
            case 3:
                return ""a few"";
            default:                  
                return ""lots"";
        }
    }
}
", 1)]
    [InlineData(@"using System;

public class A
{
    public string Run(int number)
    {
        switch (number) // +1
        {
            case 1:
                if (true) // +2 (N=1)
                    return ""one"";
                return ""ONE"";
            case 2:                   
                return ""a couple"";
            case 3:
                return ""a few"";
            default:                  
                return ""lots"";
        }
    }
}
", 3)]

    [InlineData(@"using System;

public class A
{
    public void Run(bool a, bool b)
    {
        try
        {
            if (a) // +1
            {
                
                for (int i = 0; i < 10; i++) // +2 (N=1)
                {
                    
                    while (b) // +3 (N=2)
                    {
                    } 
                }
            }
        }
        catch (Exception e) // +1
        {
            if (b) // +2 (N=1)
            {
            }
        }
    }
}
", 9)]
    [InlineData(@"using System;

public class A
{
    public void Run()
    {
        if (true) // +1
        {
            try { throw new Exception(""ErrorType1""); }
            catch (IndexOutOfRangeException ex) // +2 (N=1)
            {
            }
        }
    }
}
", 3)]
    [InlineData(@"using System;

public class A
{
    public void Run()
    {
        if (true) // +1
        {
            try { throw new Exception(""ErrorType1""); }
            catch (Exception ex) when (ex.Message == ""ErrorType2"") // +2+1 (N=1)
            {
            }
        }
    }
}
", 4)]
    [InlineData(@"using System;

public class A
{
    public void Run()
    {
        if (true) // +1
        {
            try { throw new Exception(""ErrorType1""); }
            catch (Exception ex) // +2 (N=1)
            {
                if (ex.Message == ""ErrorType3"") // +3 (N=2)
                {
                }
            }
        }
    }
}
", 6)]
    public void Complexity(string code, int expectComplexity)
    {
        var tree = CSharpSyntaxTree.ParseText(code);
        var unitRoot = tree.GetCompilationUnitRoot();
        var compilation = CSharpCompilation.Create(null, tree.ToSingleEnumerable());

        var walker = new ComplexityWalker(compilation);

        var method = unitRoot
            .DescendantNodes(node => !(node is MethodDeclarationSyntax))
            .OfType<MethodDeclarationSyntax>()
            .Single();

        var complexity = walker.Complexity(method);

        complexity.Should().Be(expectComplexity, code);
    }
}