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
            return new TestArrayExpression([.. elements.Select(e => new TestNumberExpression(e))]);
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

    public interface IParentExpression
    {
        void AssignParent();
    }

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

    public record TestFunctionCallExpression(string name, params TestExpression[] arguments) : TestExpression, IParentExpression
    {
        public string Name { get; } = name;

        public override string ToString(TestExpression expression)
        {
            return ReferenceEquals(expression, this) ? "X" : $"#({string.Join(", ", arguments.Select(s => s.ToString(expression)))})";
        }

        public void AssignParent()
        {
            foreach (var argument in arguments)
            {
                argument.Parent = this;

                if (argument is IParentExpression parentExpression)
                    parentExpression.AssignParent();
            }
        }

        public override void Assert(QxExpression expression)
        {
            var positionDescription = GetPositionDescription(this);

            var functionCallExpression = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType<QxFunctionCallExpression>(expression, $"\r\n{positionDescription}\r\nExpression was not an identifier expression.");

            if (arguments.Length != functionCallExpression.Arguments.Count)
                throw new AssertFailedException($"Function did not have {arguments.Length} arguments. It had {functionCallExpression.Arguments.Count}.");

            foreach (var (argument, argumentExpression) in arguments.Zip(functionCallExpression.Arguments))
                argument.Assert(argumentExpression);
        }
    }

    public record TestArrayExpression(TestExpression[] Elements) : TestExpression
    {
        public override string ToString(TestExpression expression)
        {
            if (ReferenceEquals(this, expression))
                return "X";

            return $"[{string.Join(",", Elements.Select(e => e.GetPositionDescription(expression)))}]";
        }

        public override void Assert(QxExpression expression)
        {
            var positionDescription = GetPositionDescription(this);

            var arrayExpression = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType<QxArrayExpression>(expression, $"\r\n{positionDescription}\r\nExpression was not an array expression.");

            var actual = arrayExpression.Elements;
            List<TestExpression> expected = [.. Elements];

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.HasCount(actual.Count, expected, $"\r\n{positionDescription}\r\nActual array had {(actual.Count > expected.Count ? "more than" : "less than")} expected number of elements.");

            foreach (var (expectedElement, actualElement) in expected.Zip(actual))
                expectedElement.Assert(actualElement);
        }

    }

    public record TestIndexerExpression(TestExpression target, TestExpression index) : TestExpression, IParentExpression
    {
        public void AssignParent()
        {
            target.Parent = this;
            index.Parent = this;

            if (target is IParentExpression targetParent)
                targetParent.AssignParent();

            if (index is IParentExpression indexParent)
                indexParent.AssignParent();
        }

        public override string ToString(TestExpression expression)
        {
            if (ReferenceEquals(this, expression))
                return "X";

            return $"{target.ToString(expression)}[{index.ToString(expression)}]";
        }

        public override void Assert(QxExpression expression)
        {
            var positionDescription = GetPositionDescription(this);

            var indexerExpression = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType<QxIndexerExpression>(expression, $"\r\n{positionDescription}\r\nExpression was not an indexer expression.");

            target.Assert(indexerExpression.Target);
            index.Assert(indexerExpression.Index);
        }
    }
}
