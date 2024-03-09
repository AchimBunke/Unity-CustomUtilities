using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;



#if USE_UNIRX_7_1
using UniRx;
using UnityUtilities.Dictionary;

namespace UnityUtilities.Reactive
{
    public static class UniRxExtensions
    {
        /// <summary>
        /// Observes any change on the Dicitonary and notifies with Unit parameter.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public static IObservable<Unit> ObserveAnyChange<TKey, TValue>(this IReadOnlyReactiveDictionary<TKey, TValue> dictionary)
        {
            return dictionary.ObserveAdd().Select(x => Unit.Default).Merge(
                dictionary.ObserveRemove().Select(x => Unit.Default),
                dictionary.ObserveReplace().Select(x => Unit.Default),
                dictionary.ObserveReset().Select(x => Unit.Default));
        }
        /// <summary>
        /// Creates an Observer for a ReactiveDictionary which updates its value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observer"></param>
        /// <returns></returns>
        public static IObserver<T> AsObserver<T>(this IReactiveProperty<T> observer)
        {
            return Observer.Create<T>(newVal => observer.Value = newVal);
        }

        private class ReactiveBoundObservableCollection<T> : ObservableCollection<T>
        {
            public ReactiveBoundObservableCollection(IEnumerable<T> sourceCollection) : base(sourceCollection) { }

            public CompositeDisposable Disposables { get; } = new CompositeDisposable();
        }
        private class ReactiveBoundObservableDictionary<TKey, TValue> : ObservableDictionary<TKey, TValue>
        {
            public ReactiveBoundObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> sourceCollection) : base(sourceCollection) { }

            public CompositeDisposable Disposables { get; } = new CompositeDisposable();
        }
        public static ObservableCollection<TValue> ToObservableCollection<TKey, TValue>(this IReadOnlyReactiveDictionary<TKey, TValue> sourceCollection)
        {
            var collection = new ReactiveBoundObservableCollection<TValue>(sourceCollection.Select(kv => kv.Value));
            collection.Disposables.Add(sourceCollection.ObserveAdd().Subscribe(evt =>
            {
                collection.Add(evt.Value);
            }));
            collection.Disposables.Add(sourceCollection.ObserveRemove().Subscribe(evt =>
            {
                collection.Remove(evt.Value);
            }));
            collection.Disposables.Add(sourceCollection.ObserveReset().Subscribe(evt =>
            {
                collection.Clear();
            }));
            collection.Disposables.Add(sourceCollection.ObserveReplace().Subscribe(evt =>
            {
                collection.Remove(evt.OldValue);
                collection.Add(evt.NewValue);
            }));
            return collection;
        }
        public static ObservableDictionary<TKey, TValue> ToObservableDictionary<TKey, TValue>(this IReadOnlyReactiveDictionary<TKey, TValue> sourceCollection)
        {
            var collection = new ReactiveBoundObservableDictionary<TKey,TValue>(sourceCollection);
            collection.Disposables.Add(sourceCollection.ObserveAdd().Subscribe(evt =>
            {
                collection.Add(evt.Key, evt.Value);
            }));
            collection.Disposables.Add(sourceCollection.ObserveRemove().Subscribe(evt =>
            {
                collection.Remove(evt.Key);
            }));
            collection.Disposables.Add(sourceCollection.ObserveReset().Subscribe(evt =>
            {
                collection.Clear();
            }));
            collection.Disposables.Add(sourceCollection.ObserveReplace().Subscribe(evt =>
            {
                collection[evt.Key] = evt.NewValue;
            }));
            return collection;
        }
        public static ObservableCollection<T> ToObservableCollection<T>(this IReadOnlyReactiveCollection<T> sourceCollection)
        {
            var collection = new ReactiveBoundObservableCollection<T>(sourceCollection);
            collection.Disposables.Add(sourceCollection.ObserveAdd().Subscribe(evt =>
            {
                collection.Add(evt.Value);
            }));
            collection.Disposables.Add(sourceCollection.ObserveRemove().Subscribe(evt =>
            {
                collection.Remove(evt.Value);
            }));
            collection.Disposables.Add(sourceCollection.ObserveReset().Subscribe(evt =>
            {
                collection.Clear();
            }));
            collection.Disposables.Add(sourceCollection.ObserveReplace().Subscribe(evt =>
            {
                collection.Remove(evt.OldValue);
                collection.Add(evt.NewValue);
            }));
            return collection;
        }
    }
}
#endif
