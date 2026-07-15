using Quixotic.Common.Statements;

namespace Quixotic.Common.Expressions
{
    public class QxFunctionDeclarationExpression(string returnType) : QxExpression
    {
        public string ReturnType { get; } = returnType;

        public List<QxParameter> Parameters { get; init; } = [];

        public Block Body { get; init; } = [];
    }
}
