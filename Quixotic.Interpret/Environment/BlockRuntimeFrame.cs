using Quixotic.Interpret.Contracts;
using Quixotic.Interpret.Exceptions;

namespace Quixotic.Interpret.Environment
{
    public class BlockRuntimeFrame(RuntimeFrameType type, IRuntimeFrame? parent = null) : IRuntimeFrame
    {
        public IRuntimeFrame? Parent { get; init; } = parent;

        public RuntimeFrameType Type { get; } = type;

        public Scope Scope { get; } = new(parent?.Scope);

        public Scope GlobalScope => Parent?.GlobalScope ?? throw new RuntimeException($"{nameof(BlockRuntimeFrame)} cannot resolve global scope.");
    }
}
