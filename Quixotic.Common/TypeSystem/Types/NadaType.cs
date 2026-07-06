using System.ComponentModel;

namespace Quixotic.Common.TypeSystem.Types
{
    [Description("nada")]
    public class NadaType : QxValueType
    {
        public static NadaType Instance { get; } = new();

        protected NadaType()
            : base("nada", typeof(object))
        { }
    }
}
