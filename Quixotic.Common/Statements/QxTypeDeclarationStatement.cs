namespace Quixotic.Common.Statements
{
    public class QxTypeDeclarationStatement(string name) : QxStatement
    {
        public string Name { get; init; } = name;

        public Block Body { get; init; } = [];
    }
}
