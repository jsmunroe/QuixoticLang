using Quixotic.Parsing.Statements;
using System.Collections;

namespace Quixotic.Parsing
{
    public class Block : IList<Statement>
    {
        private readonly List<Statement> _statements = [];

        public Statement this[int index]
        {
            get => _statements[index];
            set => _statements[index] = value;
        }

        public int Count => _statements.Count;

        public bool IsReadOnly => false;

        public void Add(Statement item)
        {
            _statements.Add(item);
        }

        public void Clear()
        {
            _statements.Clear();
        }

        public bool Contains(Statement item)
        {
            return _statements.Contains(item);
        }

        public void CopyTo(Statement[] array, int arrayIndex)
        {
            _statements.CopyTo(array, arrayIndex);
        }

        public int IndexOf(Statement item)
        {
            return _statements.IndexOf(item);
        }

        public void Insert(int index, Statement item)
        {
            _statements.Insert(index, item);
        }

        public bool Remove(Statement item)
        {
            return _statements.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _statements.RemoveAt(index);
        }

        #region IEnumerable members
        public IEnumerator<Statement> GetEnumerator()
        {
            return _statements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
