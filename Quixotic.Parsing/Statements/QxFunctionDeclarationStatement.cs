using Quixotic.Parsing.Expressions;

namespace Quixotic.Parsing.Statements
{

    public class QxFunctionDeclarationStatement(string name) : QxStatement
    {
        public string Name { get; } = name;

        public List<QxParameter> Parameters { get; init; } = [];

        public Block Body { get; init; } = [];
    }
}
