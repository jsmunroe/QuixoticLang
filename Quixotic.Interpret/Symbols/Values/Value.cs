using Quixotic.Interpret.Symbols.Instances;
using Quixotic.Common.Types;

namespace Quixotic.Interpret.Symbols.Values
{
    public abstract class Value(QxType type) : Instance(type)
    {
        public abstract object? Unwrap();
    }
}
