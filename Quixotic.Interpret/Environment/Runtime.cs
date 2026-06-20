using Quixotic.Interpret.Contracts;
using Quixotic.Interpret.Exceptions;
using Quixotic.Interpret.Symbols;

namespace Quixotic.Interpret.Environment
{
    public class Runtime : IRuntime
    {
        protected readonly Stack<IRuntimeFrame> _frames = [];

        public Runtime()
        {
            Frame = new GlobalRuntimeFrame();
            _frames.Push(Frame);
        }

        public IRuntimeFrame Frame { get; private set; }

        public virtual IRuntimeFrame PushBlock(RuntimeFrameType type)
        {
            var frame = new BlockRuntimeFrame(type, Frame);

            _frames.Push(frame);

            return Frame = frame;
        }

        public virtual IRuntimeFrame PushFunction()
        {
            var frame = new FunctionRuntimeFrame(Frame);

            _frames.Push(frame);

            return Frame = frame;
        }

        public virtual IRuntimeFrame Pop()
        {
            if (_frames.Count == 0)
                throw new RuntimeException("Execution is attempting to end frames when no frame is active.");

            var result = _frames.Pop();

            Frame = _frames.Peek();

            return result;
        }

        public virtual void ExecutePrint(Value value)
        {
            Console.WriteLine(value.Unwrap());
        }
    }
}
