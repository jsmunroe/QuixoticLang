using Quixotic.Interpret.Symbols.Instances;
using Quixotic.Interpret.Symbols.Types;

namespace Quixotic.Interpret.Symbols.Values
{
    public abstract class Reference(QxType type) : Instance(type)
    {
        private static long _nextId = 0;

        private readonly long _referenceId = Interlocked.Increment(ref _nextId);

        public override bool Equals(Instance other)
        {
            return other is Reference reference && reference._referenceId == _referenceId;
        }

        public override int GetHashCode()
        {
            return _referenceId.GetHashCode();
        }

        public bool ReferenceEquals(Reference? other)
        {
            return other?._referenceId == _referenceId;
        }
    }
}
