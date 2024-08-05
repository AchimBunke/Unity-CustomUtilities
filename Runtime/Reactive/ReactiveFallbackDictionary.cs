using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if USE_UNIRX_7_1
using UniRx;

namespace UnityUtilities.Reactive
{
    /// <summary>
    /// A <see cref="ReactiveFallbackDictionary{TKey, TValue}"/> is a readonly dictionary collection with 2 underlying ReactiveDictionaries.
    /// Querying data will proritise the <see cref="BaseDictionary"/>.
    /// If an entry is not in the base it will continue searching in the <see cref="FallbackDictionary"/>.
    /// It propagates all streams from base and fallback.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ReactiveFallbackDictionary<TKey, TValue> : IReadOnlyReactiveDictionary<TKey, TValue>, IDisposable
    {

        private class FallbackEqualityComparer : IEqualityComparer<KeyValuePair<TKey, TValue>>
        {
            public bool Equals(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            {
                return x.Key.Equals(y.Key);
            }

            public int GetHashCode(KeyValuePair<TKey, TValue> obj)
            {
                return obj.Key.GetHashCode();
            }
        }

        [NonSerialized]
        private ReactiveDictionary<TKey, TValue> _baseDictionary;
        public ReactiveDictionary<TKey, TValue> BaseDictionary
        {
            get => _baseDictionary;
            set
            {
                if (_baseDictionary == value)
                    return;
                var old = _baseDictionary;
                if (_baseDictionary != null)
                {
                    DisposeBaseSubscriptions();
                }
                _baseDictionary = value;
                if (_baseDictionary != null)
                {
                    _baseAddSubscription = _baseDictionary.ObserveAdd().Subscribe(OnBaseAdd);
                    _baseCountSubscription = _baseDictionary.ObserveCountChanged().Subscribe(OnCountChanged);
                    _baseRemoveSubscription = _baseDictionary.ObserveRemove().Subscribe(OnBaseRemove);
                    _baseReplaceSubscription = _baseDictionary.ObserveReplace().Subscribe(OnBaseReplace);
                    _baseResetSubscription = _baseDictionary.ObserveReset().Subscribe(OnReset);
                }
                OnReset(UniRx.Unit.Default);
            }
        }

        [NonSerialized]
        IReadOnlyReactiveDictionary<TKey, TValue> _fallbackDictionary;
        public IReadOnlyReactiveDictionary<TKey, TValue> FallbackDictionary
        {
            get { return _fallbackDictionary; }
            set
            {
                if (_fallbackDictionary == value)
                    return;
                var old = _fallbackDictionary;
                if (_fallbackDictionary != null)
                {
                    DisposeFallbackSubscriptions();
                }
                _fallbackDictionary = value;
                if (CheckForCircularDependency(value))
                {
                    throw new ArgumentException("Circular dependency detected in FallbackDictionaries!".);
                }
                if (_fallbackDictionary != null)
                {
                   
                    _fallbackAddSubscription = _fallbackDictionary.ObserveAdd().Subscribe(OnFallbackAdd);
                    _fallbackCountSubscription = _fallbackDictionary.ObserveCountChanged().Subscribe(OnCountChanged);
                    _fallbackRemoveSubscription = _fallbackDictionary.ObserveRemove().Subscribe(OnFallbackRemove);
                    _fallbackReplaceSubscription = _fallbackDictionary.ObserveReplace().Subscribe(OnFallbackReplace);
                    _fallbackResetSubscription = _fallbackDictionary.ObserveReset().Subscribe(OnReset);
                }
                OnReset(UniRx.Unit.Default);
            }
        }
        private bool CheckForCircularDependency(IReadOnlyReactiveDictionary<TKey,TValue> fallbackDictionary)
        {
            HashSet<IReadOnlyReactiveDictionary<TKey, TValue>> visitedDictionaries = new HashSet<IReadOnlyReactiveDictionary<TKey, TValue>>();
            while (fallbackDictionary is ReactiveFallbackDictionary<TKey, TValue> current)
            {
                if (visitedDictionaries.Contains(current))
                    return true;
                visitedDictionaries.Add(current);
                fallbackDictionary = current.FallbackDictionary;
            }
            return false;
        }

        [NonSerialized]
        FallbackEqualityComparer _fallbackEqualityComparer = new();

        public ReactiveFallbackDictionary()
        {
        }
        public ReactiveFallbackDictionary(ReactiveDictionary<TKey, TValue> baseDictionary, IReadOnlyReactiveDictionary<TKey, TValue> fallbackDictionary = null) : this()
        {
            _baseDictionary = baseDictionary;
            _fallbackDictionary = fallbackDictionary;
        }


        #region IDisposable Support
        void DisposeSubject<TSubject>(ref Subject<TSubject> subject)
        {
            if (subject != null)
            {
                try
                {
                    subject.OnCompleted();
                }
                finally
                {
                    subject.Dispose();
                    subject = null;
                }
            }
        }
        void DisposeFallbackSubscriptions()
        {
            _fallbackAddSubscription?.Dispose();
            _fallbackCountSubscription?.Dispose();
            _fallbackRemoveSubscription?.Dispose();
            _fallbackReplaceSubscription?.Dispose();
            _fallbackResetSubscription?.Dispose();
        }
        void DisposeBaseSubscriptions()
        {
            _baseAddSubscription?.Dispose();
            _baseCountSubscription?.Dispose();
            _baseRemoveSubscription?.Dispose();
            _baseReplaceSubscription?.Dispose();
            _baseResetSubscription?.Dispose();
        }
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisposeSubject(ref countChanged);
                    DisposeSubject(ref collectionReset);
                    DisposeSubject(ref dictionaryAdd);
                    DisposeSubject(ref dictionaryRemove);
                    DisposeSubject(ref dictionaryReplace);

                    DisposeFallbackSubscriptions();
                    DisposeBaseSubscriptions();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion


        public TValue this[TKey index]
        {
            get
            {
                if (_baseDictionary.TryGetValue(index, out var value))
                    return _baseDictionary[index];
                else
                    return _fallbackDictionary[index];
            }
        }

        public int Count => (_baseDictionary?.Count ?? 0) + (_fallbackDictionary?.Count(entry => !_baseDictionary.ContainsKey(entry.Key)) ?? 0);

        public bool ContainsKey(TKey key)
        {
            return (_baseDictionary?.ContainsKey(key) ?? false) || (_fallbackDictionary?.ContainsKey(key) ?? false);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            HashSet<TKey> yieldedBaseKeys = new HashSet<TKey>(_baseDictionary.Count);
            if (_baseDictionary != null)
            {
                foreach (var kvp in _baseDictionary)
                {
                    yieldedBaseKeys.Add(kvp.Key);
                    yield return kvp;
                }
            }
            if (_fallbackDictionary != null)
            {
                foreach (var kvp in _fallbackDictionary)
                {
                    if (!yieldedBaseKeys.Contains(kvp.Key))
                    {
                        yield return kvp;
                    }
                }
            }
        }

        private void OnBaseAdd(DictionaryAddEvent<TKey, TValue> evt)
        {
            if (_fallbackDictionary?.TryGetValue(evt.Key, out var oldValue) ?? false)
            {
                dictionaryReplace?.OnNext(new DictionaryReplaceEvent<TKey, TValue>(evt.Key, oldValue, evt.Value));
            }
            else
                dictionaryAdd?.OnNext(evt);
        }
        private void OnFallbackAdd(DictionaryAddEvent<TKey, TValue> evt)
        {
            if (_baseDictionary?.ContainsKey(evt.Key) ?? false)
                return;
            dictionaryAdd?.OnNext(evt);
        }
        [NonSerialized]
        IDisposable _baseAddSubscription;
        [NonSerialized]
        IDisposable _fallbackAddSubscription;
        [NonSerialized]
        Subject<DictionaryAddEvent<TKey, TValue>> dictionaryAdd = null;
        public IObservable<DictionaryAddEvent<TKey, TValue>> ObserveAdd()
        {
            if (dictionaryAdd == null)
            {
                dictionaryAdd = new Subject<DictionaryAddEvent<TKey, TValue>>();
                _baseAddSubscription = _baseDictionary?.ObserveAdd()?.Subscribe(OnBaseAdd);
                _fallbackAddSubscription = _fallbackDictionary?.ObserveAdd()?.Subscribe(OnFallbackAdd);
            }
            return dictionaryAdd;
        }

        public void OnCountChanged(int newCount)
        {
            int currentCount = this.Count;
            if (currentCount != this._oldCount)
            {
                countChanged?.OnNext(currentCount);
                _oldCount = currentCount;
            }
        }
        [NonSerialized]
        private int _oldCount;
        [NonSerialized]
        IDisposable _baseCountSubscription;
        [NonSerialized]
        IDisposable _fallbackCountSubscription;
        [NonSerialized]
        Subject<int> countChanged = null;
        public IObservable<int> ObserveCountChanged(bool notifyCurrentCount = false)
        {
            if (countChanged == null)
            {
                _oldCount = 0;
                countChanged = new Subject<int>();
                _baseCountSubscription = _baseDictionary?.ObserveCountChanged()?.Subscribe(OnCountChanged);
                _fallbackCountSubscription = _fallbackDictionary?.ObserveCountChanged()?.Subscribe(OnCountChanged);
            }
            if (notifyCurrentCount)
            {
                return countChanged.StartWith(() => this.Count);
            }
            else
            {
                return countChanged;
            }
        }

        private void OnBaseRemove(DictionaryRemoveEvent<TKey, TValue> evt)
        {
            if (_fallbackDictionary?.TryGetValue(evt.Key, out var newValue) ?? false)
            {
                dictionaryReplace?.OnNext(new DictionaryReplaceEvent<TKey, TValue>(evt.Key, evt.Value, newValue));
            }
            else
                dictionaryRemove?.OnNext(evt);
        }
        private void OnFallbackRemove(DictionaryRemoveEvent<TKey, TValue> evt)
        {
            if (_baseDictionary?.ContainsKey(evt.Key) ?? false)
                return;
            dictionaryRemove?.OnNext(evt);
        }
        [NonSerialized]
        IDisposable _baseRemoveSubscription;
        [NonSerialized]
        IDisposable _fallbackRemoveSubscription;
        [NonSerialized]
        Subject<DictionaryRemoveEvent<TKey, TValue>> dictionaryRemove = null;
        public IObservable<DictionaryRemoveEvent<TKey, TValue>> ObserveRemove()
        {
            if (dictionaryRemove == null)
            {
                dictionaryRemove = new Subject<DictionaryRemoveEvent<TKey, TValue>>();
                _baseRemoveSubscription = _baseDictionary?.ObserveRemove()?.Subscribe(OnBaseRemove);
                _fallbackRemoveSubscription = _fallbackDictionary?.ObserveRemove()?.Subscribe(OnFallbackRemove);
            }
            return dictionaryRemove;
        }

        private void OnBaseReplace(DictionaryReplaceEvent<TKey, TValue> evt)
        {
            dictionaryReplace?.OnNext(evt);
        }
        private void OnFallbackReplace(DictionaryReplaceEvent<TKey, TValue> evt)
        {
            if (_baseDictionary?.ContainsKey(evt.Key) ?? false)
                return;
            dictionaryReplace?.OnNext(evt);
        }

        [NonSerialized]
        IDisposable _baseReplaceSubscription;
        [NonSerialized]
        IDisposable _fallbackReplaceSubscription;
        [NonSerialized]
        Subject<DictionaryReplaceEvent<TKey, TValue>> dictionaryReplace = null;
        public IObservable<DictionaryReplaceEvent<TKey, TValue>> ObserveReplace()
        {
            if (dictionaryReplace == null)
            {
                dictionaryReplace = new Subject<DictionaryReplaceEvent<TKey, TValue>>();
                _baseReplaceSubscription = _baseDictionary?.ObserveReplace()?.Subscribe(OnBaseReplace);
                _fallbackReplaceSubscription = _fallbackDictionary?.ObserveReplace()?.Subscribe(OnFallbackReplace);
            }
            return dictionaryReplace;
        }

        private void OnReset(UniRx.Unit evt)
        {
            collectionReset?.OnNext(evt);
        }

        [NonSerialized]
        IDisposable _baseResetSubscription;
        [NonSerialized]
        IDisposable _fallbackResetSubscription;
        [NonSerialized]
        Subject<UniRx.Unit> collectionReset = null;
        public IObservable<UniRx.Unit> ObserveReset()
        {
            if (collectionReset == null)
            {
                collectionReset = new Subject<UniRx.Unit>();
                _baseResetSubscription = _baseDictionary?.ObserveReset()?.Subscribe(OnReset);
                _fallbackResetSubscription = _fallbackDictionary?.ObserveReset()?.Subscribe(OnReset);
            }
            return collectionReset;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (_baseDictionary?.TryGetValue(key, out value) ?? false)
                return true;
            if (_fallbackDictionary?.TryGetValue(key, out value) ?? false)
                return true;
            value = default;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns if the key points to a value int the fallback dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsFallbackKey(TKey key)
        {
            return !(_baseDictionary?.TryGetValue(key, out _) ?? false);
        }
    }

}
#endif
