﻿using System;
using BlowinCleanCode.Model;
using BlowinCleanCode.Model.Comment;
using BlowinCleanCode.Model.Comment.CommentProvider;
using BlowinCleanCode.Model.Settings;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature.Base
{
    public abstract class FeatureSymbolAnalyzeBase<TSymbol> : IFeature
        where TSymbol : ISymbol
    {
        protected AnalyzerSettings Settings => AnalyzerSettings.Instance;
        
        public abstract DiagnosticDescriptor DiagnosticDescriptor { get; }

        protected SkipAnalyze AnalyzerCommentSkipCheck => new SkipAnalyze(DiagnosticDescriptor, CommentProvider.Instance);

        protected abstract SymbolKind SymbolKind { get; }

        public void Register(AnalysisContext context) => context.RegisterSymbolAction(AnalyzeWithCheck, SymbolKind);

        private void AnalyzeWithCheck(SymbolAnalysisContext context)
        {
            if(!(context.Symbol is TSymbol s))
                return;
            
            if(AnalyzerCommentSkipCheck.Skip(context.Symbol, context.CancellationToken))
                return;
            
            Analyze(context, s);
        }
        
        protected abstract void Analyze(SymbolAnalysisContext context, TSymbol symbol);

        protected void ReportDiagnostic(SymbolAnalysisContext context, Location location)
            => ReportDiagnostic(context, location, Array.Empty<object>());
        
        protected void ReportDiagnostic(SymbolAnalysisContext context, Location location, params object[] args)
        {
            var diagnostic = Diagnostic.Create(DiagnosticDescriptor, location, args);
            context.ReportDiagnostic(diagnostic);
        }
    }
}