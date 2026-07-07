using Quixotic.Common.Contracts;
using Quixotic.Common.Environment;
using Quixotic.Common.TypeSystem;
using Quixotic.Common.TypeSystem.Symbols;
using Quixotic.Common.TypeSystem.Types;
using Quixotic.Runtime.References;
using Quixotic.Runtime.Values;

namespace Quixotic.Runtime.BuiltIn
{
    public class ArrayMethods : IFunctionProvider
    {
        public void Register(FunctionRegistry registry)
        {
            registry.Register("length", GetLength, QxType.Number, Param("array", QxType.Array(QxType.Any)));
        }

        public static Instance GetLength(Instance instance)
        {
            var array = ArrayReference.Convert(instance);

            return new NumberValue(array.Elements.Length);
        }

        private static Parameter Param(string name, QxType type) => new(name, type);
    }
}
