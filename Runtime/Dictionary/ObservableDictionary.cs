using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

public class ObservableDictionary<TKey,TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged
{
    public class NotifyDictionaryChangedEventArgs : NotifyCollectionChangedEventArgs
    {
        public NotifyDictionaryChangedEventArgs(NotifyCollectionChangedAction action) : base(action)
        {
        }
        public NotifyDictionaryChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems) : base(action, changedItems)
        {
        }
        public NotifyDictionaryChangedEventArgs(NotifyCollectionChangedAction action, object changedItem) : base(action, changedItem)
        {
        }
        public NotifyDictionaryChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems) : base(action, newItems, oldItems)
        {
        }
        public NotifyDictionaryChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex) : base(action, changedItems, startingIndex)
        {
        }
        public NotifyDictionaryChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index) : base(action, changedItem, index)
        {
        }
        public NotifyDictionaryChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem) : base(action, newItem, oldItem)
        {
        }
        public NotifyDictionaryChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex) : base(action, newItems, oldItems, startingIndex)
        {
        }
        public NotifyDictionaryChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index) : base(action, newItem, oldItem, index)
        {
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> NewPairs => NewItems.Cast<KeyValuePair<TKey, TValue>>();
        public IEnumerable<KeyValuePair<TKey, TValue>> OldPairs => OldItems.Cast<KeyValuePair<TKey, TValue>>();
    }

    public ObservableDictionary(): base() { }
    public ObservableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
    public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection) { }
    public ObservableDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }
    public ObservableDictionary(int capacity) : base(capacity) { }
    public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }
    public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer) : base(collection, comparer) { }
    public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public new void Add(TKey key, TValue value)
    {
        base.Add(key, value);
        CollectionChanged?.Invoke(this, new NotifyDictionaryChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
    }
    public new TValue this[TKey key]
    {
        get => base[key];
        set
        {
            var replaced = TryGetValue(key, out var oldValue);
            base[key] = value;
            if (replaced)
                CollectionChanged?.Invoke(this, new NotifyDictionaryChangedEventArgs(
                    NotifyCollectionChangedAction.Replace,
                    new KeyValuePair<TKey, TValue>(key, value),
                    new KeyValuePair<TKey, TValue>(key, oldValue)));
            else
                CollectionChanged?.Invoke(this, new NotifyDictionaryChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
        }
    }
    public new void Clear()
    {
        base.Clear();
        CollectionChanged?.Invoke(this, new NotifyDictionaryChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
    public new bool Remove(TKey key)
    {
        TryGetValue(key, out var toRemove);
        var removed = base.Remove(key);
        if (removed)
            CollectionChanged?.Invoke(this, new NotifyDictionaryChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, toRemove)));
        
        return removed;
    }
    public new bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value) 
    {
        var removed = base.Remove(key, out value);
        if (removed)
            CollectionChanged?.Invoke(this, new NotifyDictionaryChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value)));
        
        return removed;
    }
    public new bool TryAdd(TKey key, TValue value)
    {
        var added = base.TryAdd(key, value);
        if(added)
            CollectionChanged?.Invoke(this, new NotifyDictionaryChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
        return added;
    }

}
