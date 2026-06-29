using Quixotic.Common.Exceptions.Interpret;

namespace Quixotic.Interpret.Symbols
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
