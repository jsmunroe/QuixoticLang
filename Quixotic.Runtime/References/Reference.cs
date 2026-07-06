using Quixotic.Common.Types;
using Quixotic.Runtime.Instances;

namespace Quixotic.Runtime.References
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
