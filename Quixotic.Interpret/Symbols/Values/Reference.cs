using Quixotic.Interpret.Symbols.Types;

namespace Quixotic.Interpret.Symbols.Values
{
    public abstract record Reference
    {
        protected Reference(QxType type)
        {
            Type = type;
        }

        public QxType Type { get; }

        public bool IsNada => this is NadaValue;


    }
}
