using Quixotic.Common.Tokens;

namespace Quixotic.Common.Analysis.Expressions
{
    public class AnalysisInfo(Span span)
    {
        public Span Span { get; } = span;
    }
}
