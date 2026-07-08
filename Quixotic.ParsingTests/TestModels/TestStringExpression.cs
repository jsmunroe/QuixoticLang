using Quixotic.Common.Expressions;

namespace Quixotic.ParsingTests.TestModels
{
    public record TestStringExpression(string Value) : TestExpression
    {
        public string Value { get; } = Value;

        public override string ToString(TestExpression expression)
        {
            return ReferenceEquals(expression, this) ? "'X'" : "''";
        }

        public override void Assert(QxExpression expression)
        {
            var positionDescription = GetPositionDescription(this);

            var stringLiteralExpression = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType<QxStringLiteralExpression>(expression, $"\r\n{positionDescription}\r\nExpression was not a string literal expression.");

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(stringLiteralExpression.Value, Value, $"\r\n{positionDescription}\r\nExpected number value was {Value}, but actual number value was {stringLiteralExpression.Value}.");
        }

    }
}
