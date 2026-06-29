using Quixotic.Interpret.Symbols.Values;
using System.ComponentModel;

namespace Quixotic.Interpret.Symbols.Types
{
    [Description("number")]
    public class NumberType : QxValueType
    {
        public static NumberType Instance { get; } = new();

        protected NumberType()
            : base("number", typeof(NumberValue))
        { }

        public override bool IsTruthy(Value value)
        {
            if (value is not NumberValue numberValue)
                return false;

            return numberValue.Value != 0;
        }
    }
}
