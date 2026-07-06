using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols
{
    public class FunctionSignatureSymbol(string name, QxType returnType, params QxType[] parameterTypes) : Symbol
    {
        public string Name { get; } = name;

        public Signature Signature { get; } = new(name, parameterTypes);

        public QxType ReturnType { get; } = returnType;

        public List<QxType> ParameterTypes { get; init; } = [.. parameterTypes];

        public FunctionSignatureSymbol ReplaceGenerics(params QxType[] arguments)
        {
            var genericTypes = new Dictionary<string, QxType>();

            foreach (var (parameter, argument) in ParameterTypes.Zip(arguments))
                QxGeneric.GetKeyValues(parameter, argument, genericTypes);

            var replacedParameters = ParameterTypes.Select(p => QxGeneric.SetKeyValues(p, genericTypes));

            var replacedReturnType = QxGeneric.SetKeyValues(ReturnType, genericTypes);

            return new FunctionSignatureSymbol(Name, replacedReturnType, [.. replacedParameters]);
        }

        public override string ToString()
        {
            return $"{Name}({string.Join(", ", ParameterTypes)})";
        }
    }
}
