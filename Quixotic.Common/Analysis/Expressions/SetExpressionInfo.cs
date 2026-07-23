using Quixotic.Common.Expressions;
using Quixotic.Common.TypeSystem.Types;

namespace Quixotic.Common.Analysis.Expressions
{
    public class SetExpressionInfo(QxType elementType, QxExpression expression) : CollectionExpressionInfo(elementType, QxType.Set.MakeGenericType(elementType), expression)
    {
        protected override IEnumerable<AnalysisInfo> GetChildren()
        {
            return [.. Elements];
        }
    }
}
