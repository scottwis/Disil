//===================================================================
//Copyright (C) 2011 Scott Wisniewski (scott@scottdw2.com)
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of
//this software and associated documentation files (the "Software"), to deal in
//the Software without restriction, including without limitation the rights to
//use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//of the Software, and to permit persons to whom the Software is furnished to do
//so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//===================================================================

using System;
using System.Collections;
using System.Collections.Generic;
namespace disil.Collections
{
    internal class ReadOnlyDictionary<K, V> : IDictionary<K,V>
    {
        private const string s_cantModify = "An attempt was made to modify a ReadonlyDictionary.";

        private readonly IDictionary<K, V> m_dict;

        public ReadOnlyDictionary(IDictionary<K,V> dictionary)
        {
            m_dict = dictionary.AssumeArgNotNull("dictionary");
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return m_dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<K, V> item)
        {
            throw new NotSupportedException(s_cantModify);
        }

        public void Clear()
        {
            throw new NotSupportedException(s_cantModify);
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            return m_dict.Contains(item);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            m_dict.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            throw new NotSupportedException(s_cantModify);
        }

        public int Count
        {
            get { return m_dict.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool ContainsKey(K key)
        {
            return m_dict.ContainsKey(key);
        }

        public void Add(K key, V value)
        {
            throw new NotSupportedException(s_cantModify);
        }

        public bool Remove(K key)
        {
            throw new NotSupportedException(s_cantModify);
        }

        public bool TryGetValue(K key, out V value)
        {
            return m_dict.TryGetValue(key, out value);
        }

        public V this[K key]
        {
            get { return m_dict[key]; }
            set { throw new NotSupportedException(s_cantModify); }
        }

        public ICollection<K> Keys
        {
            get { return m_dict.Keys; }
        }

        public ICollection<V> Values
        {
            get { return m_dict.Values; }
        }
    }
}
