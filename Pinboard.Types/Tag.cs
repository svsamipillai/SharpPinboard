using System.Collections.Generic;
using System.Collections;
using System;

namespace Pinboard.Types
{

    public class Tags : IList<string>
    {
        private readonly IList<string> _list = new List<string>();

        public Tags(string Tags)
        {
            _list.AddRange(Tags.Split(' '));
        }

        public Tags(ICollection<string> Tags)
        {
            _list.AddRange(Tags);
        }

        #region Implementation of IEnumerable

        public IEnumerator<string> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<string>

        public void Add(string item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(string item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(string item)
        {
            return _list.Remove(item);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return _list.IsReadOnly; }
        }

        #endregion

        #region Implementation of IList<string>

        public int IndexOf(string item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public string this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        #endregion

        #region Tag specific features

        public override string ToString()
        {
            return String.Join("+", _list);
        }

        #endregion
    }


    public static class IListHelper
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null) throw new ArgumentNullException("list");
            if (items == null) throw new ArgumentNullException("items");
            foreach (T item in items) list.Add(item);
        }
    }

}