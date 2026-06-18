using Quixotic.Interpret.Environment;
using Quixotic.Interpret.Values;

namespace Quixotic.InterpretTests.TestImplementations
{
    internal class TestRuntime : Runtime
    {
        public List<RuntimeFrame> AllFrames { get; } = [];

        public List<Value> PrintExecutions { get; } = [];

        public override RuntimeFrame Push(RuntimeFrameType type)
        {
            var frame = base.Push(type);
            AllFrames.Add(frame);
            return frame;
        }

        public override void ExecutePrint(Value value)
        {
            base.ExecutePrint(value);

            PrintExecutions.Add(value);
        }

    }
}
