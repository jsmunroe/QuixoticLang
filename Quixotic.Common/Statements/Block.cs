using System.Collections;

namespace Quixotic.Common.Statements
{
    public class Block : IList<QxStatement>
    {
        private readonly List<QxStatement> _statements = [];

        public QxStatement this[int index]
        {
            get => _statements[index];
            set => _statements[index] = value;
        }

        public int Count => _statements.Count;

        public bool IsReadOnly => false;

        public void Add(QxStatement item)
        {
            _statements.Add(item);
        }

        public void Clear()
        {
            _statements.Clear();
        }

        public bool Contains(QxStatement item)
        {
            return _statements.Contains(item);
        }

        public void CopyTo(QxStatement[] array, int arrayIndex)
        {
            _statements.CopyTo(array, arrayIndex);
        }

        public int IndexOf(QxStatement item)
        {
            return _statements.IndexOf(item);
        }

        public void Insert(int index, QxStatement item)
        {
            _statements.Insert(index, item);
        }

        public bool Remove(QxStatement item)
        {
            return _statements.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _statements.RemoveAt(index);
        }

        public static Block Empty => [];

        #region IEnumerable members
        public IEnumerator<QxStatement> GetEnumerator()
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
