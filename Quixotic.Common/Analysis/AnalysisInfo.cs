using Quixotic.Common.Tokens;

namespace Quixotic.Common.Analysis
{
    public abstract class AnalysisInfo(Span span)
    {
        public Span Span { get; } = span;

        public AnalysisInfo? Parent { get; set; }

        public IReadOnlyList<AnalysisInfo> Children => [.. GetChildren().Where(c => c is not null)!];

        protected abstract IEnumerable<AnalysisInfo?> GetChildren();

        protected void ConnectParents()
        {
            foreach (var child in Children)
            {
                child.Parent = this;
                child.ConnectParents();
            }
        }
    }
}
