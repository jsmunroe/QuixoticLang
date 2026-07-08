using Quixotic.Common.Expressions;

namespace Quixotic.ParsingTests.TestModels
{
    public record TestCollectionExpression(TestExpression[] Elements) : TestExpression
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

            var arrayExpression = Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType<QxCollectionExpression>(expression, $"\r\n{positionDescription}\r\nExpression was not an array expression.");

            var actual = arrayExpression.Elements;
            List<TestExpression> expected = [.. Elements];

            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.HasCount(actual.Count, expected, $"\r\n{positionDescription}\r\nActual array had {(actual.Count > expected.Count ? "more than" : "less than")} expected number of elements.");

            foreach (var (expectedElement, actualElement) in expected.Zip(actual))
                expectedElement.Assert(actualElement);
        }

    }
}
