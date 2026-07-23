using Quixotic.Common.Contracts;
using Quixotic.Common.Namespaces;
using Quixotic.Common.Syntax.Casing;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Quixotic.Common.TypeSystem
{
    [DebuggerDisplay("{FullName,nq}")]
    public class TypeName
    {
        private static readonly Regex _genericNames = new(@"{([a-z0-9_]+)}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public TypeName(string name)
        {
            (Namespace, Name) = Namespace.FromFullName(name);
        }

        public TypeName(Namespace @namespace, string name)
        {
            Name = name;
            Namespace = @namespace;
        }

        public string Name { get; }

        public Namespace Namespace { get; } = Namespace.Global;

        public string FullName => (Namespace.IsGlobal) ? Name : $"{Namespace}.{Name}";

        public override bool Equals(object? obj)
        {
            return obj is TypeName typeName && Equals(typeName) || obj is string name && CaseRule.Current.Equals(FullName, name);
        }

        public bool Equals(TypeName typeName)
        {
            if (!Namespace.Equals(typeName.Namespace))
                return false;

            return IsMatch(typeName.Name);
        }

        public override int GetHashCode()
        {
            return CaseRule.Current.StringComparer.GetHashCode(FullName);
        }

        public override string ToString()
        {
            return FullName;
        }

        public static implicit operator TypeName(string name)
        {
            return new TypeName(name);
        }

        public static implicit operator string(TypeName typeName)
        {
            return typeName.Name;
        }

        public static bool IsGeneric(TypeName name)
        {
            return _genericNames.IsMatch(name);
        }

        public bool IsMatch(string second)
        {
            if (CaseRule.Current.TypeNames.Equals(Name, second))
                return true;

            var match = _genericNames.Match(this);
            if (match.Success)
            {
                var schema = CleanForRegex(this);
                var pattern = $"^{_genericNames.Replace(schema, "(.*)")}$";

                return Regex.IsMatch(second, pattern, CaseRule.Current.RegexOptions);
            }

            match = _genericNames.Match(second);
            if (match.Success)
            {
                var pattern = $"^{_genericNames.Replace(Regex.Escape(this), "(.*)")}$";

                return Regex.IsMatch(this, pattern, CaseRule.Current.RegexOptions);
            }

            return false;
        }

        private string CleanForRegex(string text)
        {
            return Regex.Escape(text).Replace("\\{", "{");
        }

        public GenericBindings GetGenericBindings(TypeName absolute, ITypeRegistry typeRegistry)
        {
            var matches = _genericNames.Matches(this);

            GenericBindings bindings = new();

            foreach (Match match in matches)
            {
                var key = match.Groups[1].Value;

                var pattern = $"^{CleanForRegex(this).Replace(CleanForRegex($"{{{key}}}"), "(.*)")}$";

                var valueMatch = Regex.Match(absolute, pattern, CaseRule.Current.RegexOptions);
                if (!valueMatch.Success)
                    throw new Exception($"A type name could not be extracted from '{absolute}' using the schema '{this}'.");

                var value = valueMatch.Groups[1].Value;

                if (!typeRegistry.TryResolve(value, out var type))
                    throw new Exception($"A type name could not be resolved for the type name '{value}'.");

                if (!bindings.TryBind(key, type))
                    throw new Exception($"The generic key '{key}' already references a type other than '{value}'.");
            }

            return bindings;
        }
    }
}
