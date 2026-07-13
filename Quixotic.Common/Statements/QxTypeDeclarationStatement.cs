namespace Quixotic.Common.Statements
{
    public class QxTypeDeclarationStatement(string name) : QxStatement
    {
        public string Name { get; init; } = name;

        public string BaseName { get; init; } = "any";

        public Block Body { get; init; } = [];
    }
}
