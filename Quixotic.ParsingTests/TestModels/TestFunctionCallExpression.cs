using Quixotic.Common.Expressions;

namespace Quixotic.ParsingTests.TestModels
{
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
}
