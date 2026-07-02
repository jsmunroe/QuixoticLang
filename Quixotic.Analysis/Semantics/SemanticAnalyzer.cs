using Quixotic.Analysis.Environment;
using Quixotic.Common.Expressions;
using Quixotic.Common.Statements;
using Quixotic.Common.Types;
using Quixotic.Common.Utilities;
using Quixotic.Parsing;

namespace Quixotic.Analysis.Semantics
{

    public class SemanticAnalyzer
    {
        private readonly SymbolTable _symbolTable = new();

        private readonly static MethodIndexer<Action<SemanticAnalyzer, QxStatement>> _statementIndexer = new(typeof(SemanticAnalyzer), "Analyze");

        private readonly static MethodIndexer<Func<SemanticAnalyzer, QxExpression, QxType>> _expressionIndexer = new(typeof(SemanticAnalyzer), "Analyze");

        public void Analyze(Parser parser)
        {
            var statements = parser.Parse();
            Analyze(statements);
        }

        public void Analyze(IEnumerable<QxStatement> statements)
        {
            foreach (var statement in statements)
                Analyze(statement);
        }

        private void Analyze(QxStatement statement)
        {
            var statementType = statement.GetType();

            if (!_statementIndexer.TryGetDelegate(statementType, out var action))
                throw new NotImplementedException($"No analyzer implemented for statement type: {statementType.Name}");

            action(this, statement);
        }

        private QxType Analyze(QxExpression expression)
        {
            var expressionType = expression.GetType();

            if (!_expressionIndexer.TryGetDelegate(expressionType, out var func))
                throw new NotImplementedException($"No analyzer implemented for expression type: {expressionType.Name}");

            return func(this, expression);
        }

        // Example statement analyzers
        protected void Analyze(QxPrintStatement statement)
        {
            // Analyze the expression to get its type
            var expressionType = Analyze(statement.Expression);

            // Print statements don't have additional semantic checks
        }

        protected QxType Analyze(QxNumberLiteralExpression expression)
        {
            return QxType.Number;
        }

        protected QxType Analyze(QxStringLiteralExpression expression)
        {
            return QxType.String;
        }

        protected QxType Analyze(QxBooleanLiteralExpression expression)
        {
            return QxType.Boolean;
        }

        protected QxType Analyze(QxBinaryExpression expression)
        {
            throw new NotImplementedException();
        }
    }
}
