// ------------------------------------------------------------------------
// 参考：https://blog.wizaman.net/archives/1003
// © 2013 wizaman 's blog
// ------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnCommon
{
    [Serializable]
    public abstract class SerializableCollection<T, Collection> : ISerializationCallbackReceiver, ICollection, IEnumerable<T> where Collection : class, ICollection, IEnumerable<T>, new()
    {
        public Collection collection { get { return _collection; } set { _collection = value; } }

        private Collection _collection = null;

        [SerializeField]
        private List<T> _ = null;

        public int Count { get { return _collection.Count; } }
        public bool IsSynchronized { get { return _collection.IsSynchronized; } }
        public object SyncRoot { get { return _collection.SyncRoot; } }

        public void CopyTo(Array array, int index) { _collection.CopyTo(array, index); }

        public IEnumerator<T> GetEnumerator() { return _collection.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public SerializableCollection()
        {
            _collection = new Collection();
        }
        public SerializableCollection(Collection collection)
        {
            _collection = (collection != null) ? collection : new Collection();
        }

        public virtual void OnBeforeSerialize()
        {
            _ = new List<T>(Count);
            _.AddRange(_collection);
        }

        public virtual void OnAfterDeserialize()
        {
            onDeserialized(_);
            _ = null;
        }

        public void clearSerializableValues()
        {
            _ = null;
        }

        protected abstract void onDeserialized(List<T> values);
    }

    [Serializable]
    public class SerializableLinkedList<T> : SerializableCollection<T, LinkedList<T>>
    {
        public LinkedListNode<T> First { get { return collection.First; } }
        public LinkedListNode<T> Last { get { return collection.Last; } }

        public SerializableLinkedList() { }
        public SerializableLinkedList(IEnumerable<T> values) : base(new LinkedList<T>(values)) { }

        public bool Contains(T value) { return collection.Contains(value); }

        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value) { return collection.AddBefore(node, value); }
        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode) { collection.AddBefore(node, newNode); }
        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value) { return collection.AddAfter(node, value); }
        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode) { collection.AddAfter(node, newNode); }

        public LinkedListNode<T> AddFirst(T value) { return collection.AddFirst(value); }
        public void AddFirst(LinkedListNode<T> node) { collection.AddFirst(node); }
        public LinkedListNode<T> AddLast(T value) { return collection.AddLast(value); }
        public void AddLast(LinkedListNode<T> node) { collection.AddLast(node); }

        public void Clear() { collection.Clear(); }

        public LinkedListNode<T> Find(T value) { return collection.Find(value); }
        public LinkedListNode<T> FindLast(T value) { return collection.FindLast(value); }

        public bool Remove(T value) { return collection.Remove(value); }
        public void Remove(LinkedListNode<T> node) { collection.Remove(node); }
        public void RemoveFirst() { collection.RemoveFirst(); }
        public void RemoveLast() { collection.RemoveLast(); }

        protected override void onDeserialized(List<T> values)
        {
            Clear();
            foreach (var value in values)
            {
                AddLast(value);
            }
        }
    }

    [Serializable]
    public class SerializableQueue<T> : SerializableCollection<T, Queue<T>>
    {
        public bool Contains(T item) { return collection.Contains(item); }

        public SerializableQueue() { }
        public SerializableQueue(int capacity) : base(new Queue<T>(capacity)) { }
        public SerializableQueue(IEnumerable<T> values) : base(new Queue<T>(values)) { }

        public T Peek() { return collection.Peek(); }
        public void Enqueue(T value) { collection.Enqueue(value); }
        public T Dequeue() { return collection.Dequeue(); }
        public void Clear() { collection.Clear(); }

        public T[] ToArray() { return collection.ToArray(); }
        public void TrimExcess() { collection.TrimExcess(); }

        protected override void onDeserialized(List<T> values)
        {
            Clear();
            collection = new Queue<T>(values.Count);
            foreach (var value in values)
            {
                collection.Enqueue(value);
            }
        }
    }

    [Serializable]
    public class SerializableStack<T> : SerializableCollection<T, Stack<T>>
    {
        public bool Contains(T value) { return collection.Contains(value); }

        public SerializableStack() { }
        public SerializableStack(int capacity) : base(new Stack<T>(capacity)) { }
        public SerializableStack(IEnumerable<T> values) : base(new Stack<T>(values)) { }

        public T Peek() { return collection.Peek(); }
        public T Pop() { return collection.Pop(); }
        public void Push(T value) { collection.Push(value); }
        public void Clear() { collection.Clear(); }

        public T[] ToArray() { return collection.ToArray(); }
        public void TrimExcess() { collection.TrimExcess(); }

        protected override void onDeserialized(List<T> values)
        {
            Clear();
            collection = new Stack<T>(values.Count);

            // スタックなので逆順に詰める必要がある
            for (int i = values.Count - 1; i >= 0; --i)
            {
                Push(values[i]);
            }
        }
    }

    [Serializable]
    public class SerializableGenericCollection<T, Collection> : ISerializationCallbackReceiver, ICollection<T> where Collection : class, ICollection<T>, new()
    {
        protected T defaultValue;

        protected Collection _collection = null;

        [SerializeField]
        private List<T> _ = null;

        public Collection collection { get { return _collection; } set { _collection = value; } }

        public int Count { get { return _collection.Count; } }
        public bool IsReadOnly { get { return _collection.IsReadOnly; } }

        public SerializableGenericCollection()
        {
            _collection = new Collection();
        }
        public SerializableGenericCollection(Collection collection)
        {
            _collection = (collection != null) ? collection : new Collection();
        }

        public bool Contains(T item) { return _collection.Contains(item); }

        public virtual void Add(T item) { _collection.Add(item); }
        public bool Remove(T item) { return _collection.Remove(item); }
        public void Clear() { _collection.Clear(); }

        public void CopyTo(T[] array, int arrayIndex) { _collection.CopyTo(array, arrayIndex); }

        public IEnumerator<T> GetEnumerator() { return _collection.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return _collection.GetEnumerator(); }

        public virtual void OnBeforeSerialize()
        {
            _ = new List<T>(Count);
            _.AddRange(_collection);
        }

        public virtual void OnAfterDeserialize()
        {
            onDeserialized(_);
            _ = null;
        }

        public void clearSerializableValues()
        {
            _ = null;
        }

        protected virtual void onDeserialized(List<T> values)
        {
            Clear();
            foreach (var value in _)
            {
                Add(value);
            }
        }
    }

    [Serializable]
    public class SerializableHashSet<T> : SerializableGenericCollection<T, HashSet<T>>
    {
        public SerializableHashSet() { }
        public SerializableHashSet(IEqualityComparer<T> comparer) : base(new HashSet<T>(comparer)) { }
        public SerializableHashSet(IEnumerable<T> values) : base(new HashSet<T>(values)) { }
        public SerializableHashSet(IEnumerable<T> values, IEqualityComparer<T> comparer) : base(new HashSet<T>(values, comparer)) { }

        public IEqualityComparer<T> Comparer { get { return collection.Comparer; } }

        public int RemoveWhere(Predicate<T> predicate) { return collection.RemoveWhere(predicate); }

        public void UnionWith(IEnumerable<T> other) { collection.UnionWith(other); }
        public void ExceptWith(IEnumerable<T> other) { collection.ExceptWith(other); }
        public void IntersectWith(IEnumerable<T> other) { collection.IntersectWith(other); }
        public void SymmetricExceptWith(IEnumerable<T> other) { collection.SymmetricExceptWith(other); }

        public bool IsProperSubsetOf(IEnumerable<T> other) { return collection.IsProperSubsetOf(other); }
        public bool IsProperSupersetOf(IEnumerable<T> other) { return collection.IsProperSupersetOf(other); }
        public bool IsSubsetOf(IEnumerable<T> other) { return collection.IsSubsetOf(other); }
        public bool IsSupersetOf(IEnumerable<T> other) { return collection.IsSupersetOf(other); }
        public bool Overlaps(IEnumerable<T> other) { return collection.Overlaps(other); }
        public bool SetEquals(IEnumerable<T> other) { return collection.SetEquals(other); }

        public void TrimExcess() { collection.TrimExcess(); }

        public void CopyTo(T[] array) { collection.CopyTo(array); }
        public void CopyTo(T[] array, int index, int count) { collection.CopyTo(array, index, count); }

        public override void Add(T item)
        {
            if (!_collection.Contains(item))
            {
                _collection.Add(item);
            }
            else
            {
                if (!_collection.Contains(defaultValue))
                {
                    _collection.Add(defaultValue);
                }
                else
                {
                    DebugLogger.LogWarning($"{item}が重複しているため追加できません。");
                }
            }
        }
    }

    [Serializable]
    public class SerializablePair<TKey, TValue>
    {
        public TKey key = default(TKey);
        public TValue value = default(TValue);
    }

    [Serializable]
    public class SerializableDictionay<TKey, TValue, Entry, Collection> : ISerializationCallbackReceiver, IDictionary<TKey, TValue>
        where Entry : SerializablePair<TKey, TValue>, new()
        where Collection : class, IDictionary<TKey, TValue>, new()
    {
        public Collection collection { get { return _collection; } set { _collection = value; } }

        protected Collection _collection = new Collection();

        protected TKey defaultKey;        

        [SerializeField]
        private List<Entry> _ = null;

        public int Count { get { return _collection.Count; } }
        public bool IsReadOnly { get { return _collection.IsReadOnly; } }

        public ICollection<TKey> Keys { get { return _collection.Keys; } }
        public ICollection<TValue> Values { get { return _collection.Values; } }

        public TValue this[TKey key] { get { return _collection[key]; } set { _collection[key] = value; } }

        public SerializableDictionay()
        {
            _collection = new Collection();
        }
        public SerializableDictionay(Collection collection)
        {
            _collection = (collection != null) ? collection : new Collection();
        }

        public virtual void OnBeforeSerialize()
        {
            _ = new List<Entry>(Count);
            foreach (var pair in _collection)
            {
                var entry = new Entry();
                entry.key = pair.Key;
                entry.value = pair.Value;
                _.Add(entry);
            }
        }

        public virtual void OnAfterDeserialize()
        {
            onDeserialized(_);
            _ = null;
        }

        public void clearSerializableValues()
        {
            _ = null;
        }

        protected virtual void onDeserialized(List<Entry> entries)
        {
            Clear();
            foreach (var entry in entries)
            {
                Add(entry.key, entry.value);
            }
        }

        public bool TryGetValue(TKey key, out TValue value) { return _collection.TryGetValue(key, out value); }

        public bool Contains(KeyValuePair<TKey, TValue> item) { return _collection.Contains(item); }
        public bool ContainsKey(TKey key) { return _collection.ContainsKey(key); }

        public virtual void Add(TKey key, TValue value) { _collection.Add(key, value); }
        public void Add(KeyValuePair<TKey, TValue> item) { _collection.Add(item); }
        public bool Remove(TKey key) { return _collection.Remove(key); }
        public bool Remove(KeyValuePair<TKey, TValue> item) { return _collection.Remove(item); }
        public void Clear() { _collection.Clear(); }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) { _collection.CopyTo(array, arrayIndex); }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { throw new NotImplementedException(); }
        IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }

    }

    [Serializable]
    public class SerializableDictionay<TKey, TValue, Entry> : SerializableDictionay<TKey, TValue, Entry, Dictionary<TKey, TValue>>
        where Entry : SerializablePair<TKey, TValue>, new()
    {
        public SerializableDictionay() { }
        public SerializableDictionay(int capacity) : base(new Dictionary<TKey, TValue>(capacity)) { }
        public SerializableDictionay(IDictionary<TKey, TValue> dictionary) : base(new Dictionary<TKey, TValue>(dictionary)) { }
        public SerializableDictionay(IEqualityComparer<TKey> comparer) : base(new Dictionary<TKey, TValue>(comparer)) { }
        public SerializableDictionay(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(new Dictionary<TKey, TValue>(dictionary, comparer)) { }
        public SerializableDictionay(int capacity, IEqualityComparer<TKey> comparer) : base(new Dictionary<TKey, TValue>(capacity, comparer)) { }

        public IEqualityComparer<TKey> Comparer { get { return collection.Comparer; } }

        public bool ContainsValue(TValue value) { return collection.ContainsValue(value); }

        protected override void onDeserialized(List<Entry> entries)
        {
            Clear();
            collection = new Dictionary<TKey, TValue>(entries.Count);
            foreach (var entry in entries)
            {
                Add(entry.key, entry.value);
            }
        }

        public override void Add(TKey key, TValue value)
        {
            if (!_collection.ContainsKey(key))
            {
                _collection.Add(key, value);
            }
            else
            {
                if (!_collection.ContainsKey(defaultKey))
                {
                    _collection.Add(defaultKey, value);
                }
                else
                {
                    DebugLogger.LogWarning($"{key}が重複しているため追加できません。");
                }
            }
        }
    }

    [Serializable]
    public class SerializableSortedDictionay<TKey, TValue, Entry> : SerializableDictionay<TKey, TValue, Entry, SortedDictionary<TKey, TValue>>
        where Entry : SerializablePair<TKey, TValue>, new()
    {
        public SerializableSortedDictionay() { }
        public SerializableSortedDictionay(IDictionary<TKey, TValue> dictionary) : base(new SortedDictionary<TKey, TValue>(dictionary)) { }
        public SerializableSortedDictionay(IComparer<TKey> comparer) : base(new SortedDictionary<TKey, TValue>(comparer)) { }
        public SerializableSortedDictionay(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer) : base(new SortedDictionary<TKey, TValue>(dictionary, comparer)) { }

        public IComparer<TKey> Comparer { get { return collection.Comparer; } }

        public bool ContainsValue(TValue value) { return collection.ContainsValue(value); }
    }
}