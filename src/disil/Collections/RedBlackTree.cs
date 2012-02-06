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
    //A redblack tree implementation. This is mostly taken from
    //the wikipedia page describing red-black trees.
    //Currently "remove" is not implemented, mainly because it wasn't
    //needed. If you need "remove", please feel free to add it.
    //We use this class, rather than SortedDictionary<K,V>, inside the definition 
    //of RegionMap. That's mainly because we need to do an "upper bound"
    //lookup, in order to efficently implement RegionMap, which 
    //SortedDictionary doesn't support. 
    //
    //This class is not meant to be
    //used directly as a collection, but instead to be used as part of
    //the implemetnation of a collection class. Hence it's "lower" level 
    //interrface. It should support dictionary, set, and "region map" style 
    //collections efficently.
    internal class RedBlackTree<K, V> : IEnumerable<RedBlackTreeNode<V>>
    {
        private readonly IComparer<K> m_comparer;
        private RedBlackTreeNode<V> m_root;

        // ReSharper disable InconsistentNaming
        private readonly Func<V, K> GetKey;
        // ReSharper restore InconsistentNaming
              
        public int Count { get; private set; }

        public RedBlackTree(IComparer<K> comparer, Func<V, K> keyExtractor)
        {
            Count = 0;
            m_root = new RedBlackTreeNode<V>(true);
            m_comparer = comparer.AssumeArgNotNull("comparer");
            GetKey = keyExtractor.AssumeArgNotNull("keyExtractor");
        }

        public RedBlackTreeNode<V> Find(K key)
        {
            var current = m_root;

            while (!current.IsLeaf)
            {
                var currentKey = GetKey(current.Data);
                var comp = m_comparer.Compare(key, currentKey);
                if (comp == 0)
                {
                    return current;
                }
                
                current = comp < 0 ? current.Left : current.Right;
            }

            return current;
        }

        public bool Insert(V value)
        {
            RedBlackTreeNode<V> node;
            return Insert(value, out node);
        }

        public bool Insert(V value, out RedBlackTreeNode<V> node)
        {
            var k = GetKey(value);
            node = Find(k);

            if (!node.IsLeaf)
            {
                return false;
            }
            node.IsBlack = false;
            node.Right = new RedBlackTreeNode<V>(true);
            node.Left = new RedBlackTreeNode<V>(true);
            node.Data = value;
            node.Left.Parent = node.Right.Parent = node;
            ++Count;
            HandleInsert(node);
            return true;
        }

        private void HandleInsert(RedBlackTreeNode<V> node)
        {
            while (true)
            {
                var p = node.Parent;
                if (p == null)
                {
                    node.IsBlack = true;
                    return;
                }

                if (p.IsBlack)
                {
                    return;
                }
                var gp = p.Parent.AssumeNotNull();

                var u = gp.Left == p ? gp.Right : gp.Left;

                if (!u.IsBlack)
                {
                    p.IsBlack = u.IsBlack = true;
                    gp.IsBlack = false;
                    node = gp;
                    continue;
                }

                if (node == p.Right && p == gp.Left)
                {
                    RotateLeft(node);
                    node = p;
                }
                else if (node == p.Left && p == gp.Right)
                {
                    RotateRight(node);
                    node = p;
                }

                p = node.Parent;
                gp = p.Parent;

                p.IsBlack = true;
                gp.IsBlack = false;

                if (node == node.Parent.Left && node.Parent == gp.Left)
                {
                    RotateRight(p);
                }
                else
                {
                    RotateLeft(p);
                }

                return;
            }
        }

        private void RotateRight(RedBlackTreeNode<V> n)
        {
            var p = n.Parent;
            n.Parent = p.Parent;
            p.Parent = n;
            if (n.Parent == null)
            {
                m_root = n;
            }
            else if (n.Parent.Left == p)
            {
                n.Parent.Left = n;
            }
            else
            {
                n.Parent.Right.AssumeEquals(p);
                n.Parent.Right = n;
            }
            p.Left = n.Right;
            p.Left.Parent = p;
            n.Right = p;
        }

        private void RotateLeft(RedBlackTreeNode<V> n)
        {
            var p = n.Parent;
            n.Parent = p.Parent;
            p.Parent = n;
            if (n.Parent == null)
            {
                m_root = n;
            }
            else if (n.Parent.Left == p)
            {
                n.Parent.Left = n;
            }
            else
            {
                n.Parent.Right = n;
            }
            p.Right = n.Left;
            p.Right.Parent = p;
            n.Left = p;
        }

        public IEnumerator<RedBlackTreeNode<V>> GetEnumerator()
        {
            var s = new Stack<KeyValuePair<RedBlackTreeNode<V>, bool>>();
            var kvp = new KeyValuePair<RedBlackTreeNode<V>, bool>(m_root, false);
            s.Push(kvp);
            while (s.Count > 0)
            {
                kvp = s.Pop();
                if (!kvp.Key.IsLeaf)
                {
                    if (kvp.Value)
                    {
                        yield return kvp.Key;
                    }
                    else
                    {
                        var leftChild = new KeyValuePair<RedBlackTreeNode<V>, bool>(kvp.Key.Left, false);
                        var rightChild = new KeyValuePair<RedBlackTreeNode<V>, bool>(kvp.Key.Right, false);
                        kvp = new KeyValuePair<RedBlackTreeNode<V>, bool>(kvp.Key, true);
                        s.Push(leftChild);
                        s.Push(kvp);
                        s.Push(rightChild);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public RedBlackTreeNode<V> FindPrevious(RedBlackTreeNode<V> node)
        {
            //0. If node is null, return null
            //1. If node is not the root, and is the right child of it's parent, return it's parent.
            //2. If the node's left child is not a leaf, return it's right most non-leaf
            //3. Follow parent pointers up as long as node is the left child of it's parent. Then return node's parent.

            if (node == null)
            {
                return null;
            }
            if (node.Parent != null && node.Parent.Right == node)
            {
                return node.Parent;
            }
            if (node.Left != null && !node.Left.IsLeaf)
            {
                node = node.Left;
                while (!node.Right.IsLeaf)
                {
                    node = node.Right;
                }
                return node;
            }
            while (node.Parent != null && node.Parent.Left == node)
            {
                node = node.Parent;
            }
            return node.Parent;
        }

        public RedBlackTreeNode<V> FindNext(RedBlackTreeNode<V> node)
        {
            //0. If node == null, return null
            //1. If node.Right is not a leaf, return the the left most nont leaf child of node.Right
            //2. follow parent pointers up as long as node is the right child of it's parent. Then return node.parent.

            if (node == null)
            {
                return null;
            }

            if (node.Right != null && !node.Right.IsLeaf)
            {
                node = node.Right;
                while (!node.Left.IsLeaf)
                {
                    node = node.Left;
                }
                return node;
            }

            while (node.Parent != null && node.Parent.Right == node)
            {
                node = node.Parent;
            }
            return node.Parent;
        }
    }
}
