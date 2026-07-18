namespace Quixotic.Common.Statements
{
    public class QxImportStatement(string @namespace) : QxStatement
    {
        public string Namespace { get; } = @namespace;
    }
}
