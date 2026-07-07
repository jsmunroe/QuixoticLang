using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Runtime.Values
{
    public class NumberValue : Instance
    {
        public NumberValue(double value) : base(QxType.Number)
        {
            this["value"] = value;
        }

        public double Value
        {
            get => (double)this["value"]!;
            set => this["value"] = value;
        }
    }
}
