using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace BlowinCleanCode.Test.Settings
{
    /// <summary>
    /// Guards the rules that keep <see cref="Constant.Option"/> usable as the single place where
    /// option names are declared. A key that breaks one of these rules cannot be documented or
    /// recognized in '.editorconfig'.
    /// </summary>
    public class AnalyzerOptionTest
    {
        [Fact]
        public void Every_option_key_is_unique()
            => AllOptionKeys().Should().OnlyHaveUniqueItems();

        [Fact]
        public void Every_option_key_starts_with_the_prefix()
            => AllOptionKeys().Should().OnlyContain(key => key.StartsWith(Constant.Option.Prefix + "."));

        [Fact]
        public void Every_option_key_contains_the_id_of_its_rule()
        {
            var ids = AllDiagnosticIds();

            AllOptionKeys().Should().OnlyContain(key => ids.Any(id => key.Contains("." + id + ".")));
        }

        [Fact]
        public void There_is_at_least_one_option()
            => AllOptionKeys().Should().NotBeEmpty();

        private static string[] AllOptionKeys()
            => StringConstantsOf(typeof(Constant.Option))
                .Where(e => e.Name != nameof(Constant.Option.Prefix))
                .Select(e => (string) e.GetValue(null))
                .ToArray();

        private static string[] AllDiagnosticIds()
            => StringConstantsOf(typeof(Constant.Id))
                .Select(e => (string) e.GetValue(null))
                .ToArray();

        private static FieldInfo[] StringConstantsOf(Type type)
            => type.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(e => e.IsLiteral && e.FieldType == typeof(string))
                .ToArray();
    }
}
