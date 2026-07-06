using Quixotic.Common.Types;

namespace Quixotic.Runtime.Symbols.Values
{
    public abstract class Value(QxType type) : Instance(type)
    {
        public abstract object? Unwrap();
    }
}
