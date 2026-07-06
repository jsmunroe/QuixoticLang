using Quixotic.Common.TypeSystem.Types;
using Quixotic.Runtime.Contracts;
using Quixotic.Runtime.Environment;
using Quixotic.Runtime.Instances;
using Quixotic.Runtime.References;
using Quixotic.Runtime.Values;

namespace Quixotic.InterpretTests.TestImplementations
{
    internal class TestRuntime : Runtime.Environment.Runtime
    {
        public TestRuntime()
        {
            AllFrames.Add(Frame); // Add global frame
        }

        public List<IRuntimeFrame> AllFrames { get; } = [];

        public List<string> PrintExecutions { get; } = [];

        public override IRuntimeFrame PushBlock(RuntimeFrameType type)
        {
            var frame = base.PushBlock(type);
            AllFrames.Add(frame);
            return frame;
        }

        public override IRuntimeFrame PushFunction()
        {
            var frame = base.PushFunction();
            AllFrames.Add(frame);
            return frame;
        }

        public override void ExecutePrint(Instance instance)
        {
            base.ExecutePrint(instance);

            PrintExecutions.Add(instance.ToString() ?? "nada");
        }

        public void AssertHasPrinted(string value)
        {
            var printExecutions = PrintExecutions.Count == 0 ? "\r\nThere were no print executions." : $"\r\nPrint executions were:\r\n\t{string.Join("\r\n\t", PrintExecutions)}\r\n";

            if (!PrintExecutions.Contains(value))
                throw new AssertFailedException($"No print executions were made with the string '{value}'.{printExecutions}");
        }

        public void AssertHasPrinted(int index, string value)
        {
            if (index < 0)
                throw new AssertFailedException($"The {nameof(index)} cannot be negative.");

            if (index >= PrintExecutions.Count)
                throw new AssertFailedException($"There are no print executions at index {index}.");

            var expected = value;
            var actual = PrintExecutions[index];

            Assert.AreEqual(expected, actual, $"The actual print execution at index {index} is '{actual}'. It was not '{expected}'.");
        }


        public void AssertHasNotPrinted(string value)
        {
            var printExecutions = PrintExecutions.Count == 0 ? "\r\nThere were no print executions." : $"\r\nPrint executions were:\r\n\t{string.Join("\r\n\t", PrintExecutions)}\r\n";

            if (PrintExecutions.Contains(value))
                throw new AssertFailedException($"At least one print executions was made with the string '{value}'.{printExecutions}");
        }

        public void AssertHasNotPrinted(int index, string value)
        {
            if (index < 0)
                throw new AssertFailedException($"The {nameof(index)} cannot be negative.");

            if (index >= PrintExecutions.Count)
                throw new AssertFailedException($"There are no print executions at index {index}.");

            var expected = value;
            var actual = PrintExecutions[index];

            Assert.AreNotEqual(expected, actual, $"'{expected}' was printed at index {index}.");
        }



        public void AssertFunctionDeclared(string name)
        {
            Assert.IsTrue(AllFrames.Any(f => f.Scope.IsFunctionDeclared(name)), $"No function named '{name}' was decleared.");
        }

        public void AssertFunctionDeclared(int frameIndex, string name)
        {
            if (frameIndex < 0)
                throw new AssertFailedException($"The {nameof(frameIndex)} cannot be negative.");

            if (frameIndex >= AllFrames.Count)
                throw new AssertFailedException($"There frames executions at index {frameIndex}.");

            Assert.IsTrue(AllFrames[frameIndex].Scope.IsFunctionDeclared(name), $"No function named '{name}' was decleared.");
        }

        public void AssertVariableIsNull(string name)
        {
            List<string> otherFrameValues = [];

            for (var i = 0; i < AllFrames.Count; i++)
            {
                var frame = AllFrames[i];

                if (!frame.Scope.IsVariableDeclared(name))
                    continue;

                var value = frame.Scope.GetInstance(name);
                if (value is NadaInstance)
                    return;
                else
                    otherFrameValues.Add($"In frame #{i}, {name} = {value}.");
            }

            if (otherFrameValues.Count == 0)
                throw new AssertFailedException($"A variable named '{name}' was never defined.");
            else
                throw new AssertFailedException($"In no frame was a variable named '{name}' left as null. \r\n\t{string.Join("\r\n\t", otherFrameValues)}");
        }

        public void AssertVariableIsNull(int frameIndex, string name)
        {
            if (frameIndex < 0)
                throw new AssertFailedException($"The {nameof(frameIndex)} cannot be negative.");

            if (frameIndex >= AllFrames.Count)
                throw new AssertFailedException($"There frames executions at index {frameIndex}.");

            var frame = AllFrames[frameIndex];

            if (!frame.Scope.IsVariableDeclared(name))
                throw new AssertFailedException($"A variable named '{name}' has not been defined.");

            var variableValue = frame.Scope.GetInstance(name);

            if (variableValue is not NadaInstance)
                throw new AssertFailedException($"A variable named '{name}' was {variableValue.Type.Name} type and has a value of {variableValue}.");
        }


        public void AssertVariableHasValue(string name, string value)
        {
            List<string> otherFrameValues = [];

            for (var i = 0; i < AllFrames.Count; i++)
            {
                var frame = AllFrames[i];

                if (!frame.Scope.IsVariableDeclared(name))
                    continue;

                if (frame.Scope.GetInstance(name) is StringValue stringValue)
                {
                    if (Equals(stringValue.Value, value))
                        return;

                    otherFrameValues.Add($"In frame #{i}, {name} = '{stringValue.Value}'.");
                }
            }

            if (otherFrameValues.Count == 0)
                throw new AssertFailedException($"A variable named '{name}' was never defined.");
            else
                throw new AssertFailedException($"In no frame was a variable named '{name}' left with a value '{value}'. \r\n\t{string.Join("\r\n\t", otherFrameValues)}");
        }

        public void AssertVariableHasValue(int frameIndex, string name, string value)
        {
            if (frameIndex < 0)
                throw new AssertFailedException($"The {nameof(frameIndex)} cannot be negative.");

            if (frameIndex >= AllFrames.Count)
                throw new AssertFailedException($"There frames executions at index {frameIndex}.");

            var frame = AllFrames[frameIndex];

            if (!frame.Scope.IsVariableDeclared(name))
                throw new AssertFailedException($"A variable named '{name}' has not been defined.");

            var variableValue = frame.Scope.GetInstance(name);

            if (variableValue is not StringValue stringValue)
                throw new AssertFailedException($"A variable named '{name}' was {variableValue.Type.Name} type and has a value of {variableValue}.");

            var expected = value;
            var actual = stringValue.Value;

            if (!Equals(stringValue.Value, value))
                Assert.AreEqual(expected, actual, $"The variable named '{name}' has a value of '{actual}'. It was not '{expected}'.");
        }

        public void AssertVariableHasValue(string name, double value)
        {
            List<string> otherFrameValues = [];

            for (var i = 0; i < AllFrames.Count; i++)
            {
                var frame = AllFrames[i];

                if (!frame.Scope.IsVariableDeclared(name))
                    continue;

                if (frame.Scope.GetInstance(name) is NumberValue numberValue)
                {
                    if (Equals(numberValue.Value, value))
                        return;

                    otherFrameValues.Add($"In frame #{i}, {name} = {numberValue.Value}.");
                }
            }

            if (otherFrameValues.Count == 0)
                throw new AssertFailedException($"A variable named '{name}' was never defined.");
            else
                throw new AssertFailedException($"In no frame was a variable named '{name}' left with a value {value}. \r\n\t{string.Join("\r\n\t", otherFrameValues)}");
        }

        public void AssertVariableHasValue(int frameIndex, string name, double value)
        {
            if (frameIndex < 0)
                throw new AssertFailedException($"The {nameof(frameIndex)} cannot be negative.");

            if (frameIndex >= AllFrames.Count)
                throw new AssertFailedException($"There frames executions at index {frameIndex}.");

            var frame = AllFrames[frameIndex];

            if (!frame.Scope.IsVariableDeclared(name))
                throw new AssertFailedException($"A variable named '{name}' has not been defined.");

            var variableValue = frame.Scope.GetInstance(name);

            if (variableValue is not NumberValue numberValue)
                throw new AssertFailedException($"A variable named '{name}' was {variableValue.Type.Name} type and has a value of {variableValue}.");

            var expected = value;
            var actual = numberValue.Value;

            if (!Equals(numberValue.Value, value))
                Assert.AreEqual(expected, actual, $"The variable named '{name}' has a value of {actual}. It was not '{expected}'.");
        }

        private string? AreEqual(ArrayReference array, double[] values)
        {
            if (array.ElementType != QxType.Number)
                return "The array is not a number array.";

            if (array.Elements.Length != values.Length)
                return $"The array length is {array.Elements.Length}. This is {(array.Elements.Length > values.Length ? "greater" : "less")} than was expected length of {values.Length}.";

            for (var i = 0; i < values.Length; i++)
            {
                var element = array.Elements[i] as NumberValue;

                if (array.Elements[i] is not NumberValue numberValue)
                    return $"The array element at index {i} is not a number. It is {array.Elements[i].Type.Name}.";

                if (!Equals(numberValue.Value, values[i]))
                    return $"The array element at index {i} is {array.Elements[i]}. It was expected to be {values[i]}.";
            }

            return null;
        }

        private string? ToString<TValue>(TValue[] array)
        {
            return $"[{string.Join(", ", array.Select(e => e?.ToString()))}]";
        }
        private string? ToString(ArrayReference array)
        {
            return $"[{string.Join(", ", array.Elements.Select(e => e?.ToString()))}]";
        }

        public void AssertVariableHasValue(string name, double[] values)
        {
            List<string> otherFrameValues = [];

            for (var i = 0; i < AllFrames.Count; i++)
            {
                var frame = AllFrames[i];

                if (!frame.Scope.IsVariableDeclared(name))
                    continue;

                if (frame.Scope.GetInstance(name) is ArrayReference array && array.ElementType == QxType.Number)
                {
                    if (AreEqual(array, values) is null)
                        return;

                    otherFrameValues.Add($"In frame #{i}, {name} = {ToString(array)}.");
                }
            }

            if (otherFrameValues.Count == 0)
                throw new AssertFailedException($"A variable named '{name}' was never defined.");
            else
                throw new AssertFailedException($"In no frame was a variable named '{name}' left with a value {ToString(values)}. \r\n\t{string.Join("\r\n\t", otherFrameValues)}");
        }

        public void AssertVariableHasValue(int frameIndex, string name, double[] value)
        {
            if (frameIndex < 0)
                throw new AssertFailedException($"The {nameof(frameIndex)} cannot be negative.");

            if (frameIndex >= AllFrames.Count)
                throw new AssertFailedException($"There frames executions at index {frameIndex}.");

            var frame = AllFrames[frameIndex];

            if (!frame.Scope.IsVariableDeclared(name))
                throw new AssertFailedException($"A variable named '{name}' has not been defined.");

            var variableValue = frame.Scope.GetInstance(name);

            if (variableValue is not ArrayReference array || array.ElementType != QxType.Number)
                throw new AssertFailedException($"A variable named '{name}' was {variableValue.Type.Name} type and has a value of {variableValue}.");

            var expected = value;
            var actual = array.Elements.Select(e => (e as NumberValue)?.Value).ToArray();

            var message = AreEqual(array, value);

            if (message is null)
                return;

            throw new AssertFailedException(message);
        }
    }
}
