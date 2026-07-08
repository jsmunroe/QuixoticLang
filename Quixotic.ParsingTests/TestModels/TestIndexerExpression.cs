using Quixotic.Common.Expressions;

namespace Quixotic.ParsingTests.TestModels
{
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
