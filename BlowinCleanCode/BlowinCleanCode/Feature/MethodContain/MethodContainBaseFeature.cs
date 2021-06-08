﻿using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.MethodContain
{
    public abstract class MethodContainBaseFeature : FeatureBase<IMethodSymbol>
    {
        protected abstract string CheckContainNameWord { get; }

        protected override SymbolKind SymbolKind => SymbolKind.Method;

        protected override void Analyze(SymbolAnalysisContext context, IMethodSymbol ms)
        {
            if (!Contain(ms.Name, CheckContainNameWord)) 
                return;
            
            ReportDiagnostic(context, ms.Locations[0], ms.Name);
        }

        private static bool Contain(string name, string containValue)
        {
            int idx;
            var lastIdx = 0;

            do
            {
                idx = name.IndexOf(containValue, lastIdx, StringComparison.Ordinal);
                lastIdx = idx + 1;
                if (HasNextSmallCharacter(name, containValue, idx))
                    return true;
                
            } while (idx >= 0 && lastIdx < name.Length);

            return false;
        }

        private static bool HasNextSmallCharacter(string name, string checkWord, int position)
        {
            if (position < 0)
                return false;

            var lastValidIndex = name.Length - 1;
            var checkCharacterIndex = position + checkWord.Length;
            return lastValidIndex >= checkCharacterIndex && char.IsUpper(name[checkCharacterIndex]);
        }
    }
}