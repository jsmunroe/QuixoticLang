using Quixotic.Common.Tokens;

namespace Quixotic.Common.Contracts
{
    public interface IHasSpan
    {
        Span Span { get; }
    }
}
