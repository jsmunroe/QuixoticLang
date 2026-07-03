using Quixotic.Common.Types;

namespace Quixotic.Common.Symbols
{
    public class Signature(string name, params QxType[] parameters)
    {
        public string Name { get; } = name;

        public List<QxType> Parameters { get; } = [.. parameters];

        public bool IsCompatible(string name, params QxType[] arguments)
        {
            if (!string.Equals(Name, name, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Parameters.Count != arguments.Length)
                return false; // TODO: Handle optional parameters later

            // Check if each argument type is assignable to the parameter type
            foreach (var (param, argument) in Parameters.Zip(arguments))
            {
                if (!param.IsAssignableFrom(argument))
                    return false;
            }

            return true;
        }

        public bool IsCompatible(Signature other)
        {
            return IsCompatible(other.Name, [.. other.Parameters]);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Signature other)
                return false;

            if (!string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Parameters.Count != other.Parameters.Count)
                return false;

            if (!Parameters.SequenceEqual(other.Parameters))
                return false;

            return true;
        }

        public bool IsMatch(string name, params QxType[] parameters)
        {
            if (!string.Equals(Name, name, StringComparison.OrdinalIgnoreCase))
                return false;

            if (Parameters.Count != parameters.Length)
                return false;

            if (!Parameters.SequenceEqual(parameters))
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
