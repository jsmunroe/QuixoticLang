using Quixotic.Common.Symbols.Functions;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Runtime.References
{
    public class FunctionReference : Instance
    {
        public FunctionReference(FunctionType functionType) : base(functionType)
        { }

        public FunctionReference(QxType returnType, params Parameter[] parameters) : base(QxType.Function.MakeFunctionType(returnType, parameters))
        { }

        public FunctionReference(Instance instance) : base(instance)
        { }

        public Function Function
        {
            get => ((FunctionType)Type).GetFunction(this);
            set => ((FunctionType)Type).SetFunction(this, value);
        }
    }
}
