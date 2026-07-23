using Quixotic.Common.Syntax.Casing;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols
{
    public class Signature(string name, params QxType[] parameters)
    {
        public string Name { get; } = name;

        public List<QxType> Parameters { get; } = [.. parameters];

        public bool TryMatch(string name, QxType[] arguments, GenericBindings bindings)
        {
            if (!string.Equals(Name, name, CaseRule.Current.StringComparison))
                return false;

            if (Parameters.Count != arguments.Length)
                return false;

            foreach (var (parameter, argument) in Parameters.Zip(arguments))
            {
                if (!parameter.Match(argument, bindings))
                    return false;
            }

            return true;
        }

        public bool TryMatch(Signature other, GenericBindings bindings)
        {
            return TryMatch(other.Name, [.. other.Parameters], bindings);
        }

        public virtual Signature Substitute(GenericBindings bindings)
        {
            var parameters = Parameters.Select(p => p.Substitute(bindings));

            return new Signature(Name, [.. parameters]);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Signature other)
                return false;

            if (!string.Equals(Name, other.Name, CaseRule.Current.StringComparison))
                return false;

            if (Parameters.Count != other.Parameters.Count)
                return false;

            if (!Parameters.SequenceEqual(other.Parameters))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Name);

            foreach (var parameter in Parameters)
                hashCode.Add(parameter);

            return hashCode.ToHashCode();
        }

        public override string ToString()
        {
            return $"{Name}({string.Join(", ", Parameters)})";
        }
    }
}
