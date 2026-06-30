namespace Quixotic.Common.Expressions
{
    public class QxIdentifierExpression(string name) : QxAssignableExpression
    {
        public string Name { get; } = name;
    }
}
