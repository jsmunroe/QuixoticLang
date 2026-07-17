using Quixotic.Common.Contracts;
using System.Text.RegularExpressions;

namespace Quixotic.Common.Syntax.Casing
{
    public class CaseRule(bool ignoreCase)
    {
        public bool Equals(string? first, string? second) => StringComparer.Equals(first, second);

        public IEqualityComparer<string> StringComparer { get; } = ignoreCase ? System.StringComparer.OrdinalIgnoreCase : System.StringComparer.Ordinal;

        public StringComparison StringComparison { get; } = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        public RegexOptions RegexOptions { get; } = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;

        public ICasingPolicy Types { get; } = PascalCase;
        public ICasingPolicy Locals { get; } = CamelCase;
        public ICasingPolicy Fields { get; } = CamelCase;
        public ICasingPolicy MethodNames { get; } = CamelCase;
        public ICasingPolicy PropertyNames { get; } = CamelCase;

        public static ICasingPolicy CamelCase { get; } = new CamelCasingPolicy();
        public static ICasingPolicy PascalCase { get; } = new PascalCasingPolicy();

        public static CaseRule Current { get; } = new(false);

    }
}
