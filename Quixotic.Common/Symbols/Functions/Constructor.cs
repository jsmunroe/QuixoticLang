using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Statements;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols.Functions
{
    public class Constructor(QxType type, Block body) : BindableFunction(type, body, QxType.Void.Type, FunctionCallType.Call)
    {
        public Constructor(Constructor other)
            : this(other.BindableType, other.Body)
        {
            Base = other.Base;
        }

        public override BoundConstructor Bind(Instance instance)
        {
            if (!BindableType.IsAssignableFrom(instance.Type))
                throw new FunctionBindTypeMismatchException(BindableType, instance.Type);

            return new BoundConstructor(instance, this);
        }

        public BaseConstructor? Base { get; init; }
    }
}
