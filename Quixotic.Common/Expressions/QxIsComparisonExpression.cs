namespace Quixotic.Common.Expressions
{
    public class QxIsComparisonExpression(QxExpression instance, string typeName, string? patternIdentifier) : QxExpression
    {
        public QxExpression Instance { get; } = instance;

        public string TypeName { get; } = typeName;

        public string? PatternIdentifier { get; } = patternIdentifier;
    }
}
