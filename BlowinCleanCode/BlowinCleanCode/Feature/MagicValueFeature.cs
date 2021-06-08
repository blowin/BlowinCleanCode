﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BlowinCleanCode.Feature
{
    public sealed class MagicValueFeature : FeatureBase<IMethodSymbol>
    {
        public override DiagnosticDescriptor DiagnosticDescriptor { get; }= new DiagnosticDescriptor(
            id: Constant.Id.MagicValue,
            title: "Expression shouldn't contain magic value",
            messageFormat: "Magic value '{0}'",
            Constant.Category.SingleResponsibility, 
            DiagnosticSeverity.Warning, 
            isEnabledByDefault: true
        );

        protected override SymbolKind SymbolKind => SymbolKind.Method;
        
        protected override void Analyze(SymbolAnalysisContext context, IMethodSymbol symbol)
        {
            foreach (var reference in symbol.DeclaringSyntaxReferences)
            {
                if(!(reference.GetSyntax(context.CancellationToken) is MethodDeclarationSyntax syntax))
                    continue;

                foreach (var syntaxNode in ChildNodes(syntax))
                {
                    if (syntaxNode.IsKind(SyntaxKind.NumericLiteralExpression))
                    {
                        ReportDiagnostic(context, syntaxNode.GetLocation(), syntaxNode.ToFullString());
                    }
                }
            }
        }

        private IEnumerable<SyntaxNode> ChildNodes(MethodDeclarationSyntax syntax)
        {
            if (syntax.Body != null)
            {
                foreach (var statementSyntax in syntax.Body.Statements)
                {
                    foreach (var syntaxNode in AllChild(statementSyntax, true))
                        yield return syntaxNode;
                }
            }
            else if (syntax.ExpressionBody != null)
            {
                foreach (var syntaxNode in AllChild(syntax.ExpressionBody, true))
                    yield return syntaxNode;
            }
        }

        private IEnumerable<SyntaxNode> AllChild(SyntaxNode node, bool checkKind)
        {
            if (NeedSkip(node))
                yield break;

            foreach (var childNode in node.ChildNodes())
            {
                if (NeedSkip(childNode))
                    continue;

                if (!checkKind || VisitKind(childNode))
                {
                    yield return childNode;

                    foreach (var n2 in AllChild(childNode, false))
                        yield return n2;
                }
            }
        }

        private static bool NeedSkip(SyntaxNode node)
        {
            if (node is VariableDeclarationSyntax vds && vds.Variables.Count == 1)
                return true;

            if (node is LocalDeclarationStatementSyntax lvds && lvds.IsConst) 
                return true;
            
            return false;
        }

        private static bool VisitKind(SyntaxNode node)
        {
            switch (node.Kind())
            {
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.SubtractExpression:
                case SyntaxKind.AddExpression:
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.ReturnKeyword:
                case SyntaxKind.ArgumentList:
                case SyntaxKind.VariableDeclaration:
                case SyntaxKind.DeclarationExpression:
                    return true;
                default:
                    return false;
            }
        }
    }
}