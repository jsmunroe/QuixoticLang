using Quixotic.Common.Types;
using Quixotic.Runtime.Instances;

namespace Quixotic.Runtime.Values
{
    public abstract class Value(QxType type) : Instance(type)
    {
        public abstract object? Unwrap();
    }
}
