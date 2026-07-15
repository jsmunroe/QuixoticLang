using System.Text.RegularExpressions;

namespace Quixotic.Common.Syntax
{
    public class CaseRule(bool ignoreCase)
    {
        public static CaseRule Current { get; } = new(false);

        public IEqualityComparer<string> StringComparer { get; } = ignoreCase ? System.StringComparer.OrdinalIgnoreCase : System.StringComparer.Ordinal;

        public StringComparison StringComparison { get; } = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

        public RegexOptions RegexOptions { get; } = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
    }
}
