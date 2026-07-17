using Quixotic.Common.Symbols;
using Quixotic.Common.Syntax.Casing;

namespace Quixotic.Common.Environment
{
    public class ClosureCapture(IEnumerable<string> names)
    {
        private readonly HashSet<string> _names = new(names, CaseRule.Current.StringComparer);

        public bool CaptureAll { get; private set; } = false;

        public bool IsCaptured(Symbol symbol)
        {
            return CaptureAll || _names.Contains(symbol.Name);
        }

        public static ClosureCapture All { get; } = new([]) { CaptureAll = true };
    }
}
