namespace Quixotic.Common.Expressions
{
    public class QxIsComparisonExpression(QxExpression target, string typeName, string? patternIdentifier) : QxExpression
    {
        public QxExpression Target { get; } = target;

        public string TypeName { get; } = typeName;

        public string? PatternIdentifier { get; } = patternIdentifier;
    }
}
