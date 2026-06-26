using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {
        public static string RemoveLeadingSpaces(this string value, int count = -1)
        {
            var matches = Regex.Matches(value, @"(^\s+)\S", RegexOptions.Multiline);

            if (matches.Count == 0)
                return value;

            var minLength = matches.Cast<Match>().Where(m => !Regex.IsMatch(m.Value, @"^\s+$")).OrderBy(m => m.Length).First().Length;

            if (count > 0)
                minLength = Math.Min(count, minLength);

            return Regex.Replace(value, @$"^\s{{{minLength}}}", string.Empty);
        }
    }
}
