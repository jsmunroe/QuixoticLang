using Quixotic.Common.Expressions;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Expressions
{
    public class ExpressionErrorInfo(Exception exception, QxExpression expression) : ExpressionInfo(QxType.Nada.Type, expression)
    {
        public Exception Exception { get; } = exception;
        protected override IEnumerable<AnalysisInfo> GetChildren()
        {
            return [];
        }
    }
}
