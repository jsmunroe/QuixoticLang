using Quixotic.Common.Environment;

namespace Quixotic.Common.Contracts
{
    public interface IFunctionProvider
    {
        void Register(FunctionRegistry registry);
    }
}