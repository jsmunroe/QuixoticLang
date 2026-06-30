namespace Quixotic.Common.Expressions
{
    public class QxIndexerExpression(QxExpression target, QxExpression index) : QxAssignableExpression
    {
        public QxExpression Target { get; } = target;
        public QxExpression Index { get; } = index;
    }
}
