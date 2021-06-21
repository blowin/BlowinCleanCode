using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

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
            public const string PublicStaticField = "BCC1000";

            public const string LongMethod = "BCC2000";
            public const string ManyParametersMethod = "BCC2001";
            public const string MethodContainAnd = "BCC2002";
            public const string MethodContainOr = "BCC2003";
            public const string MagicValue = "BCC2004";
            public const string ControlFlag = "BCC2005";

            public const string ReturnNull = "BCC3000";
            public const string StaticClass = "BCC3001";
        }

        public static class Category
        {
            public const string Encapsulation = "Encapsulation";

            public const string SingleResponsibility = "Single responsibility";

            public const string GoodPractice = "Good practice";
        }
    }
}