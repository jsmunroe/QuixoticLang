using Quixotic.Common.Expressions;

namespace Quixotic.ParsingTests.TestModels
{
    public record TestIdentifierExpression(string name) : TestExpression
    {
        public string Name { get; } = name;

        public override string ToString(TestExpression expression)
        {
            return ReferenceEquals(expression, this) ? "X" : "[#]";
        }

        public override void Assert(QxExpression expression)
        {
            var positionDescription = GetPositionDescription(this);

            var identifierExpression = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType<QxIdentifierExpression>(expression, $"\r\n{positionDescription}\r\nExpression was not an identifier expression.");

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(Name, identifierExpression.Name, $"\r\n{positionDescription}\r\nExpected identifier name was '{Name}' but actual operator was '{identifierExpression.Name}'.");
        }
    }
}
