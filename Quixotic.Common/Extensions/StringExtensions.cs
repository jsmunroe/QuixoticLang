using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {
        private static Regex _rexVowelSound = new(@"^(?:[aeiouAEIOU]|h(?:on|our|eir))", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex _rexConsonantSound = new(@"^(?:user|unique|uni[v|c|t|f]|one)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string Capitalize(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return char.ToUpper(value[0]) + value[1..];
        }

        public static string PrependIndefiniteArticle(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var trimmedWord = value.Trim();

            // Regex for words that start with a vowel sound but a consonant letter (e.g., hour, honest)
            // Or start with a vowel letter but a consonant sound (e.g., university, unique, one-way)
            bool isVowelSound = _rexVowelSound.IsMatch(trimmedWord) && !_rexConsonantSound.IsMatch(trimmedWord);
            var article = isVowelSound ? "an" : "a";

            return $"{article} {trimmedWord}";
        }
    }
}
