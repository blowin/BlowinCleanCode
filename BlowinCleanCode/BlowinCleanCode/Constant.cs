﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BlowinCleanCode
{
    public static class Constant
    {
        public static class ListOf
        {
            private static readonly Lazy<HashSet<string>> IdSet = new Lazy<HashSet<string>>(() => new HashSet<string>(typeof(Constant.Id).GetFields().Select(e => (string) e.GetValue(null))), LazyThreadSafetyMode.None);
            
            public static HashSet<string> Id => IdSet.Value;
        }

        public static class Id
        {
            // Encapsulation
            public const string PublicStaticField = "BCC1000";

            // Single responsibility
            public const string CognitiveComplexity = "BCC2000";
            public const string ManyParametersMethod = "BCC2001";
            public const string MethodContainAnd = "BCC2002";
            public const string ControlFlag = "BCC2003";
            public const string MethodContainALotOfDeclaration = "BCC2004";
            public const string LongChainCall = "BCC2005";
            public const string LargeType = "BCC2006";
            public const string LargeNumberOfFields = "BCC2007";
            public const string LongLambda = "BCC2008";
            public const string LongMethod = "BCC2009";

            // Good Practice
            public const string ReturnNull = "BCC3000";
            public const string StaticClass = "BCC3001";
            public const string DisposableMemberInNonDisposable = "BCC3002";
            public const string SwitchStatementsShouldHaveAtLeast2CaseClauses ="BCC3003";
            public const string FinalizersShouldNotBeEmpty ="BCC3004";
            public const string TypeThatProvideEqualsShouldImplementIEquatable ="BCC3005";
            public const string ThreadStaticFieldsShouldNotBeInitialized ="BCC3006";
            public const string NameTooLong = "BCC3007";
            public const string UseOnlyASCIICharactersForNames = "BCC3008";

            // Code smell
            public const string NestedTernaryOperator = "BCC4000";
            public const string ComplexCondition = "BCC4001";
            public const string MagicValue = "BCC4002";
            public const string PreserveWholeObject = "BCC4003";
            public const string HollowTypeName = "BCC4004";
            public const string DeeplyNestedCode = "BCC4005";
            public const string MethodShouldNotHaveManyReturnStatements = "BCC4006";
            public const string SwitchShouldNotHaveALotOfCases = "BCC4007";
            public const string SwitchStatementsShouldNotBeNested = "BCC4008";
            public const string CatchShouldDoMoreThanRethrow = "BCC4009";
            public const string EmptyDefaultClausesShouldBeRemoved = "BCC4010";
            public const string MiddleMan = "BCC4011";
        }

        public static class Category
        {
            public const string Encapsulation = "Encapsulation";

            public const string SingleResponsibility = "Single responsibility";
            
            public const string GoodPractice = "Good practice";
            
            public const string CodeSmell = "Code smell";
        }
    }
}