using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Extension
{
    public static class SwitchStatementSyntaxExt
    {
        public static IEnumerable<CaseSwitchLabelSyntax> AllCases(this SwitchStatementSyntax self)
        {
            return self.Sections
                .SelectMany(e => e.Labels)
                .OfType<CaseSwitchLabelSyntax>();
        }
        
        public static int CountOfCases(this SwitchStatementSyntax self)
            => self.AllCases().Count();
    }
}