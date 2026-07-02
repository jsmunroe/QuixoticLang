using Quixotic.Common.Types;
using Quixotic.Interpret.Symbols.Instances;

namespace Quixotic.Interpret.Symbols.Values
{
    public class VoisInstance() : Instance(QxType.Void)
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
        public static VoisInstance Instance { get; } = new();
    }


}
