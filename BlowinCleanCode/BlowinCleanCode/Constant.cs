using Microsoft.CodeAnalysis;

namespace BlowinCleanCode
{
    public static class Constant
    {
        public static class Id
        {
            public const string PublicStaticField = "BCC1000";

            public const string LongMethod = "BCC2000";
            public const string ManyParametersMethod = "BCC2001";
            public const string MethodContainAnd = "BCC2002";
            public const string MethodContainOr = "BCC2003";
            public const string MagicValue = "BCC2004";

            public const string ReturnNull = "BCC3000";
            public const string StaticClass = "BCC3001";
        }

        public static class Category
        {
            public const string Encapsulation = "Encapsulation";

            public const string SingleResponsibility = "Single responsibility";

            public const string GoodPractice = "Good practice";
        }

        public static class Diagnostic
        {
            public static readonly DiagnosticDescriptor MethodContainAnd = new DiagnosticDescriptor(
                id: Constant.Id.MethodContainAnd,
                title: "Method shouldn't contain 'And'",
                messageFormat: "Method '{0}' contain 'And'",
                Constant.Category.SingleResponsibility, 
                DiagnosticSeverity.Warning, 
                isEnabledByDefault: true
            );
            
            public static readonly DiagnosticDescriptor MethodContainOr = new DiagnosticDescriptor(
                id: Constant.Id.MethodContainOr,
                title: "Method shouldn't contain 'Or'",
                messageFormat: "Method '{0}' contain 'Or'",
                Constant.Category.SingleResponsibility, 
                DiagnosticSeverity.Warning, 
                isEnabledByDefault: true
            );
            
            public static readonly DiagnosticDescriptor LongMethod = new DiagnosticDescriptor(Constant.Id.LongMethod, 
                title: "Method is long",
                messageFormat: "Method '{0}' too long", 
                Constant.Category.SingleResponsibility, 
                DiagnosticSeverity.Warning, 
                isEnabledByDefault: true, 
                description: "Method must be shorter");
            
            public static readonly DiagnosticDescriptor MagicValue = new DiagnosticDescriptor(Constant.Id.MagicValue,
                title: "Expression shouldn't contain magic value",
                messageFormat: "Magic value '{0}'",
                Constant.Category.SingleResponsibility, 
                DiagnosticSeverity.Warning, 
                isEnabledByDefault: true
            );
            
            public static readonly DiagnosticDescriptor ManyParametersMethod = new DiagnosticDescriptor(Constant.Id.ManyParametersMethod, 
                title: "Method has many parameters",
                messageFormat: "Method '{0}' has many parameters", 
                Constant.Category.SingleResponsibility, 
                DiagnosticSeverity.Warning, 
                isEnabledByDefault: true);
            
            public static readonly DiagnosticDescriptor PublicStaticField = new DiagnosticDescriptor(Constant.Id.PublicStaticField, 
                title: "Field must be readonly",
                messageFormat: "Field '{0}' mutable", 
                Constant.Category.Encapsulation, 
                DiagnosticSeverity.Warning, 
                isEnabledByDefault: true, 
                description: "Type names should be readonly.");
            
            public static readonly DiagnosticDescriptor StaticClass = new DiagnosticDescriptor(Constant.Id.StaticClass, 
                title: "Class can't be static",
                messageFormat: "Class '{0}' must be non static", 
                Constant.Category.GoodPractice, 
                DiagnosticSeverity.Warning, 
                isEnabledByDefault: true,
                description: "Static class is bad practice. if you can't do without a static class, use singleton pattern.");
            
            public static readonly DiagnosticDescriptor ReturnNull = new DiagnosticDescriptor(Constant.Id.ReturnNull, 
                title: "Method return null",
                messageFormat: "Return statement with null", 
                Constant.Category.GoodPractice, 
                DiagnosticSeverity.Warning, 
                isEnabledByDefault: true, 
                description: "Return null bad practice. Use null object pattern");
        }
    }
}