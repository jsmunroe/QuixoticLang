using Quixotic.Common.Types;
using Quixotic.Interpret.Contracts;
using Quixotic.Runtime.Environment;
using Quixotic.Runtime.Symbols;
using Quixotic.Runtime.Symbols.Values;

namespace Quixotic.Runtime.BuiltIn
{
    public class BuiltInFunctions : IFunctionProvider
    {
        public void Register(FunctionRegistry registry)
        {
            registry.Register("+", (StringValue left, Instance right) => left.Add(right), QxType.String, Param("left", QxType.String), Param("right", QxType.Any));
            registry.Register("+", (NumberValue left, NumberValue right) => left.Add(right), QxType.Number, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("-", (NumberValue left, NumberValue right) => left.Subtract(right), QxType.Number, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("*", (NumberValue left, NumberValue right) => left.Multiply(right), QxType.Number, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("/", (NumberValue left, NumberValue right) => left.Divide(right), QxType.Number, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("<", (NumberValue left, NumberValue right) => left.IsLessThan(right), QxType.Boolean, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("<=", (NumberValue left, NumberValue right) => left.IsLessThanOrEqualTo(right), QxType.Boolean, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register(">", (NumberValue left, NumberValue right) => left.IsGreaterThan(right), QxType.Boolean, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register(">=", (NumberValue left, NumberValue right) => left.IsGreaterThanOrEqualTo(right), QxType.Boolean, Param("left", QxType.Number), Param("right", QxType.Number));
            registry.Register("=", (Instance left, Instance right) => left.IsEqualTo(right), QxType.Boolean, Param("left", QxType.Any), Param("right", QxType.Any));
            registry.Register("!=", (Instance left, Instance right) => left.IsNotEqualTo(right), QxType.Boolean, Param("left", QxType.Any), Param("right", QxType.Any));
            registry.Register("+", (ArrayInstance left, Instance right) => left.Add(right), QxType.Array(QxType.Any), Param("left", QxType.Array(QxType.Any)), Param("right", QxType.Any));
            registry.Register("+", (ArrayInstance left, ArrayInstance right) => left.Add(right), QxType.Array(QxType.Any), Param("left", QxType.Array(QxType.Any)), Param("right", QxType.Array(QxType.Any)));
        }

        private static Parameter Param(string name, QxType type) => new(name, type);
    }
}
