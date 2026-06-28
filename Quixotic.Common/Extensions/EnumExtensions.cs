using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Quixotic.Common.Extensions
{
    public static class EnumExtensions
    {
        private static readonly Regex _enumSpace = new(@"(?<!^)(?=[A-Z])", RegexOptions.Compiled);

        public static string ToText(this Enum value)
        {
            var type = value.GetType();
            var field = type.GetField(value.ToString());

            if (field?.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() is DescriptionAttribute description)
                return description.Description;

            var text = value.ToString();
            return _enumSpace.Replace(text, " ");
        }

        public static IEnumerable<string> ToText<TEnum>(this IEnumerable<TEnum> source)
            where TEnum : Enum
        {
            foreach (var value in source)
                yield return value.ToString();
        }
    }
}
