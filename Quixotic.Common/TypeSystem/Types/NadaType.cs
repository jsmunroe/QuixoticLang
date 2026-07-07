using System.ComponentModel;

namespace Quixotic.Common.TypeSystem.Types
{
    [Description("nada")]
    public class NadaType : QxValueType
    {
        public static NadaType Default { get; } = new();
        public static Instance Value { get; } = new Instance(Default);

        protected NadaType()
            : base("nada", typeof(object))
        { }
    }
}
