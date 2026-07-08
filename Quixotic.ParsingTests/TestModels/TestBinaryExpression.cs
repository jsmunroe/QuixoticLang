using Quixotic.Common.Expressions;

namespace Quixotic.ParsingTests.TestModels
{
    public record TestBinaryExpression : TestExpression, IParentExpression
    {
        public TestBinaryExpression(TestExpression left, string op, TestExpression right)
        {
            Left = left;
            Operator = op;
            Right = right;

            AssignParent();
        }

        public TestExpression Left { get; }

        public string Operator { get; }

        public TestExpression Right { get; }

        public void AssignParent()
        {
            Left.Parent = this;
            Right.Parent = this;

            if (Left is IParentExpression left)
                left.AssignParent();

            if (Right is IParentExpression right)
                right.AssignParent();
        }

        public override void Assert(QxExpression expression)
        {
            var positionDescription = GetPositionDescription(this);

            var binaryExpression = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType<QxBinaryExpression>(expression, $"\r\n{positionDescription}\r\nExpression was not a binary expression.");

            Left.Assert(binaryExpression.Left);

            var c = GetOperatorChar(binaryExpression.Operator);

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(Operator, c, $"\r\n{positionDescription}\r\nExpected operator was '{Operator}' but actual operator was '{binaryExpression.Operator}'.");

            Right.Assert(binaryExpression.Right);
        }

        public override string ToString(TestExpression expression)
        {
            var left = ReferenceEquals(Left, expression) ? "X" : Left.ToString(expression);
            var right = ReferenceEquals(Right, expression) ? "X" : Right.ToString(expression);

            return $"({left} {Operator} {right})";
        }
    }
}
