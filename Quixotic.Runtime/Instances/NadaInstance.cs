using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Runtime.Instances
{

    public class NadaInstance() : Instance(QxType.Nada)
    {
        public override string ToString() => "nada";

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(Instance other)
        {
            return other is NadaInstance;
        }

        public override bool IsTruthy()
        {
            return false;
        }

        public static NadaInstance Instance { get; } = new();
    }


}
