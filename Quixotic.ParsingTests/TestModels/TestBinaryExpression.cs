using Quixotic.Parsing.Expressions;
using Quixotic.Parsing.Operations;
using System.Text.RegularExpressions;

namespace Quixotic.ParsingTests.TestModels
{
    public abstract record TestExpression
    {
        public TestBinaryExpression? Parent { get; set; }

        public abstract string GetPositionDescription(TestExpression expression);

        public abstract string ToString(TestExpression expression);

        public abstract void Assert(Parsing.Expressions.QxExpression expression);

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
                return new TestUnaryExpression(Parsing.Operations.Operator.Subtract, new TestNumberExpression(-value));

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

        public static implicit operator TestExpression((double left, string op, double right) tuple)
        {
            return new TestBinaryExpression((TestExpression)tuple.left, tuple.op, (TestExpression)tuple.right);
        }

        public static implicit operator TestExpression((TestBinaryExpression left, string op, double right) tuple)
        {
            return new TestBinaryExpression(tuple.left, tuple.op, (TestExpression)tuple.right);
        }

        public static implicit operator TestExpression((double left, string op, TestBinaryExpression right) tuple)
        {
            return new TestBinaryExpression((TestExpression)tuple.left, tuple.op, tuple.right);
        }

        public static implicit operator TestExpression((string left, string op, double right) tuple)
        {
            return new TestBinaryExpression((TestExpression)tuple.left, tuple.op, (TestExpression)tuple.right);
        }

        public static implicit operator TestExpression((double left, string op, string right) tuple)
        {
            return new TestBinaryExpression((TestExpression)tuple.left, tuple.op, (TestExpression)tuple.right);
        }

        public static implicit operator TestExpression((string left, string op, string right) tuple)
        {
            return new TestBinaryExpression((TestExpression)tuple.left, tuple.op, (TestExpression)tuple.right);
        }

        public static implicit operator TestExpression((TestBinaryExpression left, string op, string right) tuple)
        {
            return new TestBinaryExpression(tuple.left, tuple.op, (TestExpression)tuple.right);
        }

        public static implicit operator TestExpression((string left, string op, TestBinaryExpression right) tuple)
        {
            return new TestBinaryExpression((TestExpression)tuple.left, tuple.op, tuple.right);
        }

        public static implicit operator TestExpression((TestExpression left, string op, TestExpression right) tuple)
        {
            return new TestBinaryExpression(tuple.left, tuple.op, tuple.right);
        }
    }

    public record TestBinaryExpression : TestExpression
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

            if (Left is TestBinaryExpression left)
                left.AssignParent();

            if (Right is TestBinaryExpression right)
                right.AssignParent();
        }

        public override void Assert(Parsing.Expressions.QxExpression expression)
        {
            var positionDescription = GetPositionDescription(this);

            var binaryExpression = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType<Parsing.Expressions.QxBinaryExpression>(expression, $"\r\n{positionDescription}\r\nExpression was not a binary expression.");

            Left.Assert(binaryExpression.Left);

            var c = GetOperatorChar(binaryExpression.Operator);

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(Operator, c, $"\r\n{positionDescription}\r\nExpected operator was '{Operator}' but actual operator was '{binaryExpression.Operator}'.");

            Right.Assert(binaryExpression.Right);
        }

        public override string GetPositionDescription(TestExpression expression)
        {
            if (Parent is null)
                return ToString(expression);

            return Parent.GetPositionDescription(expression);
        }

        public override string ToString(TestExpression expression)
        {
            var left = ReferenceEquals(Left, expression) ? "X" : Left.ToString(expression);
            var right = ReferenceEquals(Right, expression) ? "X" : Right.ToString(expression);

            return $"({left} {Operator} {right})";
        }
    }

    public record TestNumberExpression(double Value) : TestExpression
    {
        public double Value { get; } = Value;

        public override string GetPositionDescription(TestExpression expression)
        {
            if (Parent is null)
                return "X";

            return Parent.GetPositionDescription(expression);
        }

        public override string ToString(TestExpression expression)
        {
            return ReferenceEquals(expression, this) ? "X" : "#";
        }

        public override void Assert(Parsing.Expressions.QxExpression expression)
        {
            var positionDescription = GetPositionDescription(this);

            var numberLiteralExpression = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType<QxNumberLiteralExpression>(expression, $"\r\n{positionDescription}\r\nExpression was not a number literal expression.");

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(numberLiteralExpression.Value, Value, $"\r\n{positionDescription}\r\nExpected number value was {Value}, but actual number value was {numberLiteralExpression.Value}.");
        }
    }

    public record TestStringExpression(string Value) : TestExpression
    {
        public string Value { get; } = Value;

        public override string GetPositionDescription(TestExpression expression)
        {
            if (Parent is null)
                return "X";

            return Parent.GetPositionDescription(expression);
        }

        public override string ToString(TestExpression expression)
        {
            return ReferenceEquals(expression, this) ? "'X'" : "''";
        }

        public override void Assert(Parsing.Expressions.QxExpression expression)
        {
            var positionDescription = GetPositionDescription(this);

            var stringLiteralExpression = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType<QxStringLiteralExpression>(expression, $"\r\n{positionDescription}\r\nExpression was not a string literal expression.");

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(stringLiteralExpression.Value, Value, $"\r\n{positionDescription}\r\nExpected number value was {Value}, but actual number value was {stringLiteralExpression.Value}.");
        }

    }

    public record TestUnaryExpression(Operator Operator, TestExpression Operand) : TestExpression
    {
        public Operator Operator { get; } = Operator;
        public TestExpression Operand { get; } = Operand;
        public override string GetPositionDescription(TestExpression expression)
        {
            if (Parent is null)
                return "X";

            return Parent.GetPositionDescription(expression);
        }

        public override string ToString(TestExpression expression)
        {
            return ReferenceEquals(expression, this) ? "X" : "-#"; ;
        }

        public override void Assert(Parsing.Expressions.QxExpression expression)
        {
            var positionDescription = GetPositionDescription(this);

            var unaryExpression = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType<Parsing.Expressions.QxUnaryExpression>(expression, $"\r\n{positionDescription}\r\nExpression was not a unary expression.");

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(Operator, unaryExpression.Operator, $"\r\n{positionDescription}\r\nExpected operator was '{Operator}' but actual operator was '{unaryExpression.Operator}'.");
            Operand.Assert(unaryExpression.Operand);
        }
    }

    public record TestIdentifierExpression(string name) : TestExpression
    {
        public string Name { get; } = name;

        public override string GetPositionDescription(TestExpression expression)
        {
            if (Parent is null)
                return "X";

            return Parent.GetPositionDescription(expression);
        }

        public override string ToString(TestExpression expression)
        {
            return ReferenceEquals(expression, this) ? "X" : "[#]";
        }

        public override void Assert(Parsing.Expressions.QxExpression expression)
        {
            var positionDescription = GetPositionDescription(this);

            var identifierExpression = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType<QxIdentifierExpression>(expression, $"\r\n{positionDescription}\r\nExpression was not an identifier expression.");

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(Name, identifierExpression.Name, $"\r\n{positionDescription}\r\nExpected identifier name was '{Name}' but actual operator was '{identifierExpression.Name}'.");
        }
    }
}
