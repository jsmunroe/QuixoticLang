using System.ComponentModel;
using System.Reflection;
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

        public static bool HasAttribute<TAttribute>(this Type type)
             where TAttribute : Attribute
        {
            return type.GetCustomAttribute<TAttribute>() is not null;
        }

        public static IEnumerable<Type> GetTypesWithAttribute<TAttribute>(this Assembly assembly)
             where TAttribute : Attribute
        {
            return assembly.GetTypes().Where(t => t.HasAttribute<TAttribute>());
        }

        public static IEnumerable<Type> GetTypesWithAttribute<TAttribute>(this AppDomain appDomain)
             where TAttribute : Attribute
        {
            return appDomain.GetAssemblies().SelectMany(a => a.GetTypesWithAttribute<TAttribute>());
        }
    }
}
