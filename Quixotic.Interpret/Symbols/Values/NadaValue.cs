using Quixotic.Interpret.Symbols.Instances;
using Quixotic.Interpret.Symbols.Types;

namespace Quixotic.Interpret.Symbols.Values
{

    public class NadaValue() : Instance(QxType.Nada)
    {
        public override string ToString() => "nada";

        public override bool Equals(object? obj)
        {
            return obj is NadaValue;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(Instance other)
        {
            return other is NadaValue;
        }

        public override bool IsTruthy()
        {
            return false;
        }

        public static NadaValue Value { get; } = new();
    }


}
