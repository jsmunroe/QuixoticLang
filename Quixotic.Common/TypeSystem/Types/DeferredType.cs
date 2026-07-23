using Quixotic.Common.Exceptions.Interpret;
using System.Diagnostics.CodeAnalysis;

namespace Quixotic.Common.TypeSystem.Types
{
    public class DeferredType(string reasonForDeffering, ContextDependency contextDependency) : QxType("deffered")
    {
        private HashSet<QxType> _alternatives = [];

        public override bool HasGenerics => false;

        public QxType NecessaryBaseType { get; init; } = Any;

        [MemberNotNullWhen(true, nameof(SelectedAlternative))]
        public bool HasAlternative => SelectedAlternative is not null;

        public IReadOnlySet<QxType> Alternatives => _alternatives.AsReadOnly();

        public QxType? SelectedAlternative { get; private set; }

        public string ReasonForDeffering { get; } = reasonForDeffering;

        public ContextDependency ContextDependency { get; } = contextDependency;

        public override bool Match(QxType actual, GenericBindings bindings)
        {
            return ReferenceEquals(this, actual);
        }

        public void AddAlternative(QxType alternativeType)
        {
            if (!NecessaryBaseType.IsAssignableFrom(alternativeType))
                throw new RuntimeException($"Alternative type '{alternativeType}' offered to deferred type must be a subclass of '{NecessaryBaseType}' and is not.");

            _alternatives.Add(alternativeType);
        }

        public QxType SelectAlternative(QxType fallback)
        {
            if (_alternatives.Count == 0)
                SelectedAlternative = fallback;
            else if (_alternatives.Count == 1)
                SelectedAlternative = _alternatives.First();
            else
                SelectedAlternative = GetCommonBase(_alternatives);

            return SelectedAlternative;
        }
    }

    public enum ContextDependency
    {
        AssignmentToMember,
        ReturnedValuesAnalyzed,
        VariableAssignment
    }
}
