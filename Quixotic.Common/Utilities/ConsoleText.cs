using Quixotic.Common.Contracts;

namespace Quixotic.Common.Utilities
{
    public class ConsoleText(string? text = null)
    {
        private List<ConsoleTextNode> _nodes = text is null ? [] : [new ConsoleTextNode { Text = text }];

        public void Clear() => _nodes.Clear();

        public void Write(object text, ConsoleColor foreground = ConsoleColor.Gray)
        {
            _nodes.Add(new()
            {
                Text = text.ToString() ?? string.Empty,
                Foreground = foreground,
            });
        }

        public void WriteLine(object text, ConsoleColor foreground = ConsoleColor.Gray)
        {
            _nodes.Add(new()
            {
                Text = text.ToString() ?? string.Empty + System.Environment.NewLine,
                Foreground = foreground,
            });
        }

        public void WriteLine()
        {
            _nodes.Add(new()
            {
                Text = System.Environment.NewLine,
            });
        }

        public void WriteWarning(object text, ConsoleColor foreground = ConsoleColor.Yellow)
        {
            _nodes.Add(new()
            {
                Text = text.ToString() ?? string.Empty,
                Foreground = foreground,
            });
        }

        public void WriteError(object text, Exception? exception = null, ConsoleColor foreground = ConsoleColor.Red)
        {
            if (exception is not null)
                text += System.Environment.NewLine + exception.ToString() + System.Environment.NewLine;

            _nodes.Add(new()
            {
                Text = text.ToString() ?? string.Empty + System.Environment.NewLine,
                Foreground = foreground
            });
        }

        class ConsoleTextNode
        {
            public ConsoleColor Foreground { get; init; }
            public ConsoleColor? Background { get; init; }

            public string Text { get; init; } = string.Empty;
        }

        public void Output(IConsoleWriter writer)
        {
            foreach (var node in _nodes)
                writer.Write(node.Text, node.Foreground);
        }

        public override string ToString()
        {
            return string.Join("", _nodes.Select(n => n.Text)).TrimEnd(' ');
        }
    }
}
