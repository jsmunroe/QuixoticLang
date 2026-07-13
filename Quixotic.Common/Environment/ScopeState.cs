namespace Quixotic.Common.Environment
{
    public class ScopeState
    {
        public FunctionRegistry Functions { get; } = new();

        public TypeRegistry Types { get; } = new();

        public VariableRegistry Variables { get; } = new();
    }
}
