using Quixotic.Common.Types;

namespace Quixotic.Common.Symbols
{
    public class FunctionTypeSymbol(QxType returnType, params QxType[] parameterTypes) : Symbol
    {
        public QxType ReturnType { get; } = returnType;

        public List<QxType> ParameterTypes { get; init; } = [.. parameterTypes];
    }
}
