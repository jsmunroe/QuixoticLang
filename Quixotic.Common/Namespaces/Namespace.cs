using Quixotic.Common.Syntax.Casing;

namespace Quixotic.Common.Namespaces
{
    public class Namespace : IEquatable<Namespace>
    {
        private readonly string _namespace;
        private readonly string[] _parts;

        public Namespace(string @namespace)
        {
            _namespace = @namespace;
            _parts = @namespace.Split('.');
            IsGlobal = @namespace == string.Empty;
        }

        public Namespace(string[] parts)
        {
            _namespace = string.Join('.', parts);
            _parts = [.. parts];
            IsGlobal = parts.Length == 0;
        }

        public Namespace? Parent => _parts.Length <= 1 ? null : new Namespace(_parts[..^1]);

        public bool IsParentOf(Namespace other)
        {
            if (other._parts.Length >= _parts.Length)
                return false;

            foreach (var (thisPart, otherPart) in _parts.Zip(other._parts))
            {
                if (!CaseRule.Current.Equals(thisPart, otherPart))
                    return false;
            }

            return true;
        }

        public bool IsChildOf(Namespace other) => other.IsParentOf(this);

        public bool IsGlobal { get; }

        public override string ToString()
        {
            return string.Join('.', _parts);
        }

        public override bool Equals(object? obj)
        {
            return obj is Namespace @namespace && Equals(@namespace) ||
                obj is string stringNamespace && CaseRule.Current.Equals(ToString(), stringNamespace);
        }

        public bool Equals(Namespace? other)
        {
            if (other is null)
                return false;

            if (IsGlobal && other.IsGlobal == true)
                return true;

            return CaseRule.Current.Equals(ToString(), other.ToString());
        }

        public override int GetHashCode()
        {
            return CaseRule.Current.StringComparer.GetHashCode(ToString());
        }

        public static Namespace Global { get; } = new(string.Empty);

        public static (Namespace, string) FromFullName(string name)
        {
            var parts = name.Split('.');

            var @namespace = new Namespace(parts[..^1]);
            name = parts[^1];

            return (@namespace, name);
        }

        public static Namespace Parse(string name)
        {
            var parts = name.Split('.');

            var @namespace = new Namespace(parts);

            return @namespace;
        }

        public static implicit operator Namespace(string @namespace)
        {
            return Parse(@namespace);
        }

        public static implicit operator string(Namespace @namespace)
        {
            return @namespace.ToString();
        }

    }
}
