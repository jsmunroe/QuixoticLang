using Quixotic.Parsing.Expressions;

namespace Quixotic.Parsing.Statements
{
    public class QxDoStatement(QxExpression condition, bool entryControlled) : QxStatement
    {
        public QxExpression Condition { get; } = condition;

        public bool EntryControlled { get; } = entryControlled;

        public bool ExitControlled { get; } = !entryControlled;

        public Block Block { get; set; } = [];
    }
}
