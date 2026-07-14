using Quixotic.Common.Expressions;
using Quixotic.Common.Tokens;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols.Functions
{
    public class BaseConstructor(QxType type, Span span)
    {
        public QxType Type { get; } = type;

        public Span Span { get; } = span;

        public List<QxExpression> Arguments { get; init; } = [];
    }
}
