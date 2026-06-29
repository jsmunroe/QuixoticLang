using System.ComponentModel;
using System.Text.RegularExpressions;

namespace System
{
    public static class TypeExctensions
    {
        private static readonly Regex _enumSpace = new(@"(?<!^)(?=[A-Z])", RegexOptions.Compiled);

        public static string Describe(this Type type)
        {
            if (type.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() is DescriptionAttribute descriptionAttribute)
                return descriptionAttribute.Description;

            return _enumSpace.Replace(type.Name, " ");
        }
    }
}
