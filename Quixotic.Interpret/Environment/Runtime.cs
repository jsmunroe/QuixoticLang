using Quixotic.Interpret.Contracts;
using Quixotic.Interpret.Exceptions;
using Quixotic.Interpret.Values;

namespace Quixotic.Interpret.Environment
{
    public class Runtime : IRuntime
    {
        protected readonly Stack<RuntimeFrame> _frames = [];

        public Runtime()
        {
            Frame = new RuntimeFrame(RuntimeFrameType.Global);
            _frames.Push(Frame);
        }

        public RuntimeFrame Frame { get; private set; }

        public virtual RuntimeFrame Push(RuntimeFrameType type)
        {
            var frame = new RuntimeFrame(type, Frame);

            _frames.Push(frame);

            return Frame = frame;
        }

        public virtual RuntimeFrame Pop()
        {
            if (_frames.Count == 0)
                throw new RuntimeException("Execution is attempting to end frames when no frame is active.");

            return _frames.Pop();
        }

        public virtual void ExecutePrint(Value value)
        {
            Console.WriteLine(value.Unwrap());
        }
    }
}
