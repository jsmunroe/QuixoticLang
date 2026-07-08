using Quixotic.Common.Expressions;
using Quixotic.Common.Operations;

namespace Quixotic.ParsingTests.TestModels
{
    public record TestUnaryExpression(Operator Operator, TestExpression Operand) : TestExpression, IParentExpression
    {
        public Operator Operator { get; } = Operator;
        public TestExpression Operand { get; } = Operand;

        public void AssignParent()
        {
            Operand.Parent = this;

            if (Operand is IParentExpression operand)
                operand.AssignParent();
        }

        public override string ToString(TestExpression expression)
        {
            return ReferenceEquals(expression, this) ? "X" : "-#"; ;
        }

        public override void Assert(QxExpression expression)
        {
            var positionDescription = GetPositionDescription(this);

            var unaryExpression = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType<QxUnaryExpression>(expression, $"\r\n{positionDescription}\r\nExpression was not a unary expression.");

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(Operator, unaryExpression.Operator, $"\r\n{positionDescription}\r\nExpected operator was '{Operator}' but actual operator was '{unaryExpression.Operator}'.");
            Operand.Assert(unaryExpression.Operand);
        }
    }
}
