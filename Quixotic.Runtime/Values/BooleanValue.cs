using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Runtime.Values
{
    public class BooleanValue : Instance
    {
        public BooleanValue(bool value) : base(QxType.Boolean)
        {
            this["value"] = value;
        }

        public bool Value
        {
            get => (bool)this["value"]!;
            set => this["value"] = value;
        }

        public static BooleanValue True => new(true);
        public static BooleanValue False => new(false);
    }
}
