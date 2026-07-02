using Quixotic.Common.Types;

namespace Quixotic.Common.Symbols
{
    public class FunctionSignatureSymbol(string name, QxType returnType, params QxType[] parameterTypes) : Symbol
    {
        public string Name { get; } = name;

        public Signature Signature { get; } = new(name, parameterTypes);

        public QxType ReturnType { get; } = returnType;

        public List<QxType> ParameterTypes { get; init; } = [.. parameterTypes];

        public override string ToString()
        {
            return $"{Name}({string.Join(", ", ParameterTypes)})";
        }
    }
}
