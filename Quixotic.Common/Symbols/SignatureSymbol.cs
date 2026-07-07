using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols
{
    public class SignatureSymbol(string name, QxType returnType, params QxType[] parameterTypes) : Symbol
    {
        public string Name { get; } = name;

        public Signature Signature { get; } = new(name, parameterTypes);

        public QxType ReturnType { get; } = returnType;

        public List<QxType> ParameterTypes { get; init; } = [.. parameterTypes];

        public SignatureSymbol ReplaceGenerics(params QxType[] arguments)
        {
            var genericTypes = new Dictionary<string, QxType>();

            foreach (var (parameter, argument) in ParameterTypes.Zip(arguments))
                Generic.GetKeyValues(parameter, argument, genericTypes);

            var replacedParameters = ParameterTypes.Select(p => Generic.SetKeyValues(p, genericTypes));

            var replacedReturnType = Generic.SetKeyValues(ReturnType, genericTypes);

            return new SignatureSymbol(Name, replacedReturnType, [.. replacedParameters]);
        }

        public override string ToString()
        {
            return $"{Name}({string.Join(", ", ParameterTypes)})";
        }
    }
}
