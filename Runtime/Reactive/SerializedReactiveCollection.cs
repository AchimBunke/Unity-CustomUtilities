using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace UnityUtilities.Reactive
{
    [Serializable]
    public class SerializedReactiveCollection<T> : ReactiveCollection<T>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<T> _serializedItems;
        public void OnAfterDeserialize()
        {
            if (_serializedItems != null)
                foreach (var i in _serializedItems)
                    Add(i);
        }

        public void OnBeforeSerialize()
        {
            if (_serializedItems == null) _serializedItems = new List<T>();
            _serializedItems.Clear();
            foreach (var item in this)
                _serializedItems.Add(item);

        }
    }
}
