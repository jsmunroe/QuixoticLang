using Quixotic.Common.Types;
using Quixotic.Interpret.Symbols.Instances;
namespace Quixotic.Interpret.Symbols.Values
{
    public class StringValue(string value) : Value(QxType.String)
    {
        public string Value { get; } = value;

        public override Instance Add(Instance right)
        {
            return new StringValue(Value + right.ToString());
        }

        public override object? Unwrap()
        {
            return Value;
        }

        public override bool Equals(Instance other)
        {
            return other is StringValue stringValue && Value == stringValue.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool IsTruthy()
        {
            return !string.IsNullOrEmpty(Value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}