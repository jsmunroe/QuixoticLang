using Quixotic.Common.Symbols.Functions;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.TypeSystem.Types
{
    public abstract class ConstructedGenericType(string name, GenericTypeDefinition definition) : QxType(name)
    {
        protected GenericTypeDefinition Definition { get; } = definition;

        public override bool TryResolveConstructor(QxType[] arguments, [NotNullWhen(true)] out Constructor? constructor)
        {
            if (base.TryResolveConstructor(arguments, out constructor))
                return true;

            if (Definition.TryResolveConstructor(arguments, out constructor))
                return true;

            return false;
        }

        public override bool TryResolveMethod(string name, QxType[] arguments, [NotNullWhen(true)] out Function? function)
        {
            if (base.TryResolveMethod(name, arguments, out function))
                return true;

            if (Definition.TryResolveMethod(name, arguments, out function))
                return true;

            return false;
        }
    }

}
