using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtilities.Dictionary;

#if USE_UNIRX_7_1
using UniRx;

namespace UnityUtilities.Reactive
{
    [Serializable]
    public class SerializedReactiveDictionary<TKey, TValue> : ReactiveDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<DictionaryEntry<TKey, TValue>> _serializedItems;
        public void OnAfterDeserialize()
        {
            if (_serializedItems != null)
                foreach (var i in _serializedItems)
                    this[i.key] = i.value;
        }

        public void OnBeforeSerialize()
        {
            if (_serializedItems == null) _serializedItems = new List<DictionaryEntry<TKey, TValue>>();
            _serializedItems.Clear();
            foreach (var item in this)
            {
                _serializedItems.Add(new DictionaryEntry<TKey, TValue>() { key = item.Key, value = item.Value });
            }
        }
    }
}
#endif