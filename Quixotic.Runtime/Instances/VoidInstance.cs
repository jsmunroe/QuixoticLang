using Quixotic.Common.Types;

namespace Quixotic.Runtime.Instances
{
    public class VoidInstance() : Instance(QxType.Void)
    {
        public override string ToString() => "void";

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(Instance other)
        {
            return false;
        }

        public override bool IsTruthy()
        {
            return false;
        }
        public static VoidInstance Instance { get; } = new();
    }


}
