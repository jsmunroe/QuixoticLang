using OmniSharp.Extensions.LanguageServer.Protocol;
using Quixotic.Analysis.Extensions;
using Quixotic.Analysis.Semantics;
using Quixotic.Analysis.Sessions;
using Quixotic.Common.Contracts;
using Quixotic.Parsing;
using QuixoticLang.Lexer;

namespace Quixotic.LanguageServer.Services
{
    public sealed class CompilationService
    {
        private readonly SemanticAnalyzer _analyzer = new();

        private readonly Dictionary<DocumentUri, AnalysisSession> _sessions = [];

        public AnalysisSession Compile(DocumentUri uri, ISource source)
        {
            var lexer = new Lexer(source);
            var parser = new Parser(lexer);
            var session = parser.ParseSession();

            _sessions[uri] = session;

            return session;
        }

        public AnalysisSession? Get(DocumentUri uri)
        {
            return _sessions.TryGetValue(uri, out var session)
                ? session
                : null;
        }

        public void Remove(DocumentUri uri)
        {
            _sessions.Remove(uri);
        }
    }
}
