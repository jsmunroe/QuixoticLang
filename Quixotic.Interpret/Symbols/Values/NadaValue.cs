using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Interpret.Symbols.Types;

namespace Quixotic.Interpret.Symbols.Values
{

    public record NadaValue() : Value(QxType.Nada, null)
    {
        public override string ToString() => "nada";

        public override Value Add(Value right)
        {
            if (right is StringValue stringValue)
                return new StringValue("nada" + stringValue.Value);

            throw new BinaryOperatorException(Type, "+", right.Type);
        }

        public static NadaValue Value { get; } = new();
    }


}
