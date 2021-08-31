using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlowinCleanCode.Extension
{
    public static class SwitchStatementSyntaxExt
    {
        public static IEnumerable<SwitchLabelSyntax> AllCases(this SwitchStatementSyntax self) 
            => self.Sections
                .SelectMany(e => e.Labels.Where(lb => lb.IsNot<DefaultSwitchLabelSyntax>()));

        public static int CountOfCases(this SwitchStatementSyntax self)
            => self.AllCases().Count();
    }
}