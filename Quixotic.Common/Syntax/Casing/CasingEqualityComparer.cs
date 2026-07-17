using Quixotic.Common.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.Syntax.Casing
{
    public class CasingEqualityComparer(ICasingPolicy casingPolicy) : IEqualityComparer<string>
    {
        public bool Equals(string? x, string? y)
        {
            if (x is null && y is null)
                return true;

            if (x is null || y is null)
                return false;

            return casingPolicy.Recase(x).Equals(casingPolicy.Recase(y));
        }

        public int GetHashCode([DisallowNull] string obj)
        {
            return casingPolicy.Recase(obj).GetHashCode();
        }
    }
}
