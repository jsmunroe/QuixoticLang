using Quixotic.Common.Environment;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Common.Expressions;
using Quixotic.Common.Statements;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Symbols.Functions
{
    public class Constructor(QxType type, Block body) : BindableFunction(type, body, QxType.Void.Type, CallType.ConstructorCall)
    {
        public Constructor(Constructor other)
            : this(other.BindableType, other.Body)
        {
            Base = other.Base;
        }

        public BaseConstructor? Base { get; init; }

        public override BoundConstructor Bind(Instance instance)
        {
            if (!BindableType.IsAssignableFrom(instance.Type))
                throw new FunctionBindTypeMismatchException(BindableType, instance.Type);

            return new BoundConstructor(instance, this);
        }

        public override Constructor Substitute(GenericBindings bindings)
        {
            var function = base.Substitute(bindings);

            return new Constructor(BindableType, Body);
        }

        public static Constructor FromDelegate(QxType bindableType, ExternalFunction implementation, params Parameter[] parameters)
        {
            var parameterTypes = parameters.Select(p => p.Type);

            List<QxExpression> parameterExpressions = [new QxIdentifierExpression("this"), .. parameters.Select(p => new QxIdentifierExpression(p.Name))];

            var externalCallExpression = new QxExternalCallExpression(implementation) { Arguments = [.. parameterExpressions] };
            var externalCallStatement = new QxExternalCallStatement(externalCallExpression);

            var constructor = new Constructor(bindableType, [externalCallStatement]) { Parameters = [.. parameters] };

            return constructor;
        }
    }
}
