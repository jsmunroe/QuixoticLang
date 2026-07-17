using Quixotic.Common.Contracts;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Quixotic.Common.Syntax.Casing
{
    public class PascalCasingPolicy : ICasingPolicy
    {
        private static readonly Regex _rexPascalCasing = new(@"^[A-Z][a-z0-9]*([A-Z][a-z0-9]*)*$");

        public CasingType Type => CasingType.Pascal;

        public string Recase(string text)
        {
            var result = JsonNamingPolicy.CamelCase.ConvertName(text);

            return result.Capitalize();
        }

        public bool IsMatch(string text)
        {
            return _rexPascalCasing.IsMatch(text);
        }

        public bool Equals(string? x, string? y)
        {
            var casingEqualityComparer = new CasingEqualityComparer(this);
            return casingEqualityComparer.Equals(x, y);
        }
    }
}
