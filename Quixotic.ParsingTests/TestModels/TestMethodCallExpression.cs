using Quixotic.Common.Expressions;

namespace Quixotic.ParsingTests.TestModels
{
    public record TestMethodCallExpression(string name, params TestExpression[] arguments) : TestExpression, IParentExpression
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

            var methodCallExpression = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType<QxMethodCallExpression>(expression, $"\r\n{positionDescription}\r\nExpression was not an identifier expression.");

            if (arguments.Length != methodCallExpression.Arguments.Count)
                throw new AssertFailedException($"Method did not have {arguments.Length} arguments. It had {methodCallExpression.Arguments.Count}.");

            foreach (var (argument, argumentExpression) in arguments.Zip(methodCallExpression.Arguments))
                argument.Assert(argumentExpression);
        }
    }
}
