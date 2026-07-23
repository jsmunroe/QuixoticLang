using Quixotic.Analysis.Contracts;
using Quixotic.Common.Analysis;
using Quixotic.Common.Analysis.Expressions;
using Quixotic.Common.Analysis.Statements;
using Quixotic.Common.Contracts;
using Quixotic.Common.Source;
using Quixotic.Common.Tokens;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Analysis.Sessions
{
    public class SourceDatabase(ISource source) : ISourceDatabase
    {
        private readonly List<SourceDatabaseEntry> _entries = [];

        private readonly SourceDocument _document = new(source);

        public void Add(StatementInfo statement)
        {
            foreach (var entry in InitEntries(statement.Span))
                entry.Statement = statement;
        }

        public void Add(ExpressionInfo expression)
        {
            foreach (var entry in InitEntries(expression.Span))
                entry.Expressions.Add(expression);
        }

        public bool TryQuery(int index, [NotNullWhen(returnValue: true)] out ISourceDatabaseEntry? entry)
        {
            entry = null;

            if (index < 0 || index >= _entries.Count)
                return false;

            entry = _entries[index];

            return true;
        }

        public bool TryQuery(int lineNumber, int column, [NotNullWhen(returnValue: true)] out ISourceDatabaseEntry? entry)
        {
            entry = null;

            var index = _document.GetIndex(lineNumber, column);

            if (index < 0)
                return false;

            return TryQuery(index, out entry);
        }

        public ISourceDatabaseEntry? Query(int index)
        {
            return TryQuery(index, out var items) ? items : null;
        }

        public ISourceDatabaseEntry? Query(int lineNumber, int column)
        {
            return TryQuery(lineNumber, column, out var items) ? items : null;
        }

        private List<SourceDatabaseEntry> InitEntries(Span span)
        {
            for (var i = _entries.Count; i <= span.End.Index; i++)
                _entries.Add(new(_document.GetPosition(i)));

            return _entries.GetRange(span.Start.Index, span.Length);
        }

        class SourceDatabaseEntry(Position position) : ISourceDatabaseEntry
        {
            public virtual StatementInfo? Statement { get; set; }

            public virtual List<ExpressionInfo> Expressions { get; set; } = [];

            public IReadOnlyList<AnalysisInfo> Items => Statement is not null
                ? [Statement, .. Expressions]
                : Expressions.AsReadOnly();

            public Position Position { get; } = position;

            public override string ToString()
            {
                return $"{Position}{(Statement is null ? string.Empty : " S")}{(Expressions.Count == 0 ? string.Empty : $" E{Expressions.Count}")}";
            }
        }
    }
}
