using Quixotic.Interpret.Symbols.Values;
using System.ComponentModel;

namespace Quixotic.Interpret.Symbols.Types
{
    [Description("boolean")]
    public class BooleanType : QxValueType
    {
        public static BooleanType Instance { get; } = new();

        protected BooleanType()
            : base("boolean", typeof(BooleanType))
        { }

        public override bool IsTruthy(Value value)
        {
            if (value is not BooleanValue booleanValue)
                return false;

            return booleanValue.Value;
        }
    }
}
