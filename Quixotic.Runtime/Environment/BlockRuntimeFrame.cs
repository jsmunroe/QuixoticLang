using Quixotic.Common.Environment;
using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Runtime.Contracts;

namespace Quixotic.Runtime.Environment
{
    public class BlockRuntimeFrame(IRuntimeFrame? parent = null) : IRuntimeFrame
    {
        public IRuntimeFrame? Parent { get; init; } = parent;

        public Scope Scope { get; } = new Scope(parent?.Scope);

        public Scope GlobalScope => Parent?.GlobalScope ?? throw new RuntimeException($"{nameof(BlockRuntimeFrame)} cannot resolve global scope.");
    }
}
