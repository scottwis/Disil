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
using System.Linq;

namespace disil.Collections
{
    internal class RegionMap<T> : IEnumerable<T>
    {
        private readonly RedBlackTree<ulong, T> m_tree;

        // ReSharper disable InconsistentNaming
        private readonly Func<T, ulong> GetAddress;
        private readonly Func<T, ulong> GetSize;
        // ReSharper restore FieldCanBeMadeReadOnly.Local

        public RegionMap(Func<T, ulong> getAddress, Func<T, ulong> getSize)
        {
            m_tree = new RedBlackTree<ulong, T>(Comparer<ulong>.Default, getAddress);
            GetAddress = getAddress;
            GetSize = getSize;
        }

        public void Add(T value)
        {
            var address = GetAddress(value);
            var size = GetSize(value);

            var node = m_tree.Find(address);
            if (!node.IsLeaf)
            {
                var prev = m_tree.FindPrevious(node);
                var next = m_tree.FindNext(node);

                if (
                    (prev != null && Overlaps(prev.Data, address, size))
                    || (next != null && Overlaps(next.Data, address, size))
                )
                {
                    throw new ArgumentException("A item covering the specified range already exists in the map.");
                }
            }
            m_tree.Insert(value);
        }

        public bool Contains(ulong address)
        {
            var node = m_tree.Find(address);
            if (node.IsLeaf)
            {
                node = m_tree.FindPrevious(node);
            }
            return !node.IsLeaf && Overlaps(node.Data, address, 1);
        }

        public T this[ulong address]
        {
            get
            {
                T ret;
                if (TryGetValue(address, out ret))
                {
                    return ret;
                }
                throw new KeyNotFoundException();
            }
        }

        public bool TryGetValue(ulong address, out T item)
        {
            var node = m_tree.Find(address);
            if (node.IsLeaf)
            {
                node = m_tree.FindPrevious(node);
            }
            if (node != null && !node.IsLeaf && Overlaps(node.Data, address, 1))
            {
                item = node.Data;
                return true;
            }
            item = default(T);
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_tree.Select(x => x.Data).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool Overlaps(T item, ulong address, ulong size)
        {
            var itemAddress = GetAddress(item);
            var itemSize = GetSize(item);

            return
                (address >= itemAddress && address < itemAddress + itemSize)
                || (itemAddress >= address && itemAddress < address + size);
        }
    }
}
