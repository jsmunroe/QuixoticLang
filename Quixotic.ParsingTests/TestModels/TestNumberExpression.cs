using Quixotic.Common.Expressions;

namespace Quixotic.ParsingTests.TestModels
{
    public record TestNumberExpression(double Value) : TestExpression
    {
        public double Value { get; } = Value;

        public override string ToString(TestExpression expression)
        {
            return ReferenceEquals(expression, this) ? "X" : "#";
        }

        public override void Assert(QxExpression expression)
        {
            var positionDescription = GetPositionDescription(this);

            var numberLiteralExpression = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType<QxNumberLiteralExpression>(expression, $"\r\n{positionDescription}\r\nExpression was not a number literal expression.");

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(numberLiteralExpression.Value, Value, $"\r\n{positionDescription}\r\nExpected number value was {Value}, but actual number value was {numberLiteralExpression.Value}.");
        }
    }
}
