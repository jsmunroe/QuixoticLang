using Quixotic.Common.Contracts;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Quixotic.Common.Syntax.Casing
{
    public class CamelCasingPolicy : ICasingPolicy
    {
        private static readonly Regex _rexCamelCasing = new(@"^[a-z][A-Za-z0-9_]*$");

        public CasingType Type => CasingType.Camel;

        public string Recase(string text)
        {
            var result = JsonNamingPolicy.CamelCase.ConvertName(text);
            return result;
        }

        public bool IsMatch(string text)
        {
            return _rexCamelCasing.IsMatch(text);
        }

        public bool Equals(string? x, string? y)
        {
            var casingEqualityComparer = new CasingEqualityComparer(this);
            return casingEqualityComparer.Equals(x, y);
        }
    }
}
