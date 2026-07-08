using Quixotic.Common.Expressions;
using Quixotic.Common.Operations;
using System.Text.RegularExpressions;

namespace Quixotic.ParsingTests.TestModels
{
    public abstract record TestExpression
    {
        public TestExpression? Parent { get; set; }

        public string GetPositionDescription(TestExpression expression)
        {
            if (Parent is null)
                return ToString(expression);

            return Parent.GetPositionDescription(expression);
        }

        public abstract string ToString(TestExpression expression);

        public abstract void Assert(QxExpression expression);

        public static TestExpression Create(TestExpression expression)
        {
            return expression;
        }

        public static string GetOperatorChar(Operator @operator)
        {
            return OperationMetadata.GetOperatorValue(@operator) ?? throw new InvalidOperationException($"Unsupported operator: {@operator}");
        }

        public static implicit operator TestExpression(double value)
        {
            if (value < 0)
                return new TestUnaryExpression(Common.Operations.Operator.Subtract, new TestNumberExpression(-value));

            return new TestNumberExpression(value);
        }


        public static implicit operator TestExpression(string value)
        {
            var identifier = new Regex(@"^\[(.+)\]$");

            var match = identifier.Match(value);
            if (match.Success)
                return new TestIdentifierExpression(match.Groups[1].Value);

            return new TestStringExpression(value);
        }


        private static TestExpression BuildBinary(TestExpression left, string op, TestExpression right)
        {
            return op == "["
                ? new TestIndexerExpression(left, right)
                : new TestBinaryExpression(left, op, right);

        }

        public static implicit operator TestExpression(bool value)
        {
            return new TestBooleanExpression(value);
        }

        public static implicit operator TestExpression(double[] elements)
        {
            return new TestCollectionExpression([.. elements.Select(e => new TestNumberExpression(e))]);
        }

        public static implicit operator TestExpression((double left, string op, double right) tuple)
        {
            return new TestBinaryExpression((TestExpression)tuple.left, tuple.op, (TestExpression)tuple.right);
        }

        public static implicit operator TestExpression((TestBinaryExpression left, string op, double right) tuple)
        {
            return BuildBinary(tuple.left, tuple.op, (TestExpression)tuple.right);
        }

        public static implicit operator TestExpression((double left, string op, TestBinaryExpression right) tuple)
        {
            return BuildBinary((TestExpression)tuple.left, tuple.op, tuple.right);
        }

        public static implicit operator TestExpression((string left, string op, double right) tuple)
        {
            return BuildBinary((TestExpression)tuple.left, tuple.op, (TestExpression)tuple.right);
        }

        public static implicit operator TestExpression((double left, string op, string right) tuple)
        {
            return BuildBinary((TestExpression)tuple.left, tuple.op, (TestExpression)tuple.right);
        }

        public static implicit operator TestExpression((string left, string op, string right) tuple)
        {
            return BuildBinary((TestExpression)tuple.left, tuple.op, (TestExpression)tuple.right);
        }

        public static implicit operator TestExpression((TestBinaryExpression left, string op, string right) tuple)
        {
            return BuildBinary(tuple.left, tuple.op, (TestExpression)tuple.right);
        }

        public static implicit operator TestExpression((string left, string op, TestBinaryExpression right) tuple)
        {
            return BuildBinary((TestExpression)tuple.left, tuple.op, tuple.right);
        }

        public static implicit operator TestExpression((TestExpression left, string op, TestExpression right) tuple)
        {
            return BuildBinary(tuple.left, tuple.op, tuple.right);
        }
    }
}
