using Quixotic.Common.Expressions;

namespace Quixotic.ParsingTests.TestModels
{
    public record TestBooleanExpression(bool Value) : TestExpression
    {
        public bool Value { get; } = Value;

        public override string ToString(TestExpression expression)
        {
            return ReferenceEquals(expression, this) ? "'X'" : "''";
        }

        public override void Assert(QxExpression expression)
        {
            var positionDescription = GetPositionDescription(this);

            var stringLiteralExpression = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType<QxBooleanLiteralExpression>(expression, $"\r\n{positionDescription}\r\nExpression was not a string literal expression.");

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(stringLiteralExpression.Value, Value, $"\r\n{positionDescription}\r\nExpected boolean value was {Value}, but actual boolean value was {stringLiteralExpression.Value}.");
        }
    }
}
