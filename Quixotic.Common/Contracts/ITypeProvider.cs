using Quixotic.Common.Environment;

namespace Quixotic.Common.Contracts
{
    public interface ITypeProvider
    {
        void Register(TypeRegistry registry);
    }
}