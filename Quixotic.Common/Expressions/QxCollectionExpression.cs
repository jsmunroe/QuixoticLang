namespace Quixotic.Common.Expressions
{
    public class QxCollectionExpression : QxExpression
    {
        public List<QxExpression> Elements { get; init; } = [];
    }
}
