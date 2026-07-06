using Quixotic.Common.Exceptions.Interpret;
using Quixotic.Runtime.Contracts;
using Quixotic.Runtime.Symbols.Values;

namespace Quixotic.Runtime.Environment
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

        public virtual void ExecutePrint(Instance value)
        {
            Console.WriteLine(value.ToString());
        }
    }
}
