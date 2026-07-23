using Quixotic.Analysis.Sessions;
using Quixotic.Common.Statements;
using Quixotic.Parsing;

namespace Quixotic.Analysis.Extensions
{
    public static class ParserExtensions
    {
        public static AnalysisSession ParseSession(this Parser parser)
        {
            var statements = parser.Parse();
            var root = new Block(statements);

            return new(parser.Source, root);

        }
    }
}
