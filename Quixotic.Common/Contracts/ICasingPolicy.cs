using Quixotic.Common.Syntax.Casing;

namespace Quixotic.Common.Contracts
{
    public interface ICasingPolicy
    {
        CasingType Type { get; }

        string Recase(string text);

        bool IsMatch(string text);

        bool Equals(string? x, string? y);
    }
}
