using System;

#if USE_UNIRX_7_1
using UniRx;

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
    }
}
#endif
