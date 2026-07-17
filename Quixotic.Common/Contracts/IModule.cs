using Quixotic.Common.Namespaces;

namespace Quixotic.Common.Contracts
{
    public interface IModule
    {
        Namespace Namespace { get; }

        ITypeProvider Types { get; }
    }
}
