using Quixotic.Interpret.Symbols.Values;
using System.ComponentModel;

namespace Quixotic.Interpret.Symbols.Types
{
    [Description("number")]
    public class NadaType : QxValueType
    {
        public static NadaType Instance { get; } = new();

        protected NadaType()
            : base("nada", typeof(NadaValue))
        { }

        public override bool IsTruthy(Value value)
        {
            return false;
        }
    }
}
