namespace Quixotic.Common.Statements
{
    public class Stepper<TItem>
    {
        private readonly IEnumerator<TItem> _tokens;
        private bool _isAtEnd = false;

        public Stepper(IEnumerable<TItem> tokens)
        {
            _tokens = tokens.GetEnumerator();
            Advance(); // Move to the first item.
        }

        public List<TItem> PastItems { get; } = [];

        public bool IsAtEnd => _isAtEnd;

        public bool Advance()
        {
            if (_isAtEnd)
                return false;

            PastItems.Add(Peek());

            _isAtEnd = !_tokens.MoveNext();
            return !_isAtEnd;
        }

        public TItem Peek()
        {
            if (IsAtEnd)
                throw new InvalidOperationException($"Cannot {nameof(Peek)} when at end of list.");

            return _tokens.Current;
        }

        public TItem Pop()
        {
            var token = Peek();
            Advance();

            return token;
        }
    }
}
