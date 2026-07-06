using Quixotic.Runtime.Environment;

namespace Quixotic.Interpret.Contracts
{
    public interface IFunctionProvider
    {
        void Register(FunctionRegistry registry);
    }
}