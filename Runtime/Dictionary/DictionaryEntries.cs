using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilities.Dictionary
{
    [Serializable]
    public struct ObjectDictionaryEntry
    {
        public ObjectDictionaryEntry(string key, UnityEngine.Object value) 
        {
            this.key = key;
            this.value = value;
        }

        [SerializeField]
        public string key;

        [SerializeField]
        public UnityEngine.Object value;
    }

    [Serializable]
    public struct StringDictionaryEntry
    {
        public StringDictionaryEntry(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        [SerializeField]
        public string key;

        [SerializeField]
        public string value;
    }

    [Serializable]
    public struct FloatDictionaryEntry
    {
        public FloatDictionaryEntry(string key, float value)
        {
            this.key = key;
            this.value = value;
        }
        [SerializeField]
        public string key;

        [SerializeField]
        public float value;
    }

    [Serializable]
    public struct BooleanDictionaryEntry
    {
        public BooleanDictionaryEntry(string key, bool value)
        {
            this.key = key;
            this.value = value;
        }
        [SerializeField]
        public string key;

        [SerializeField]
        public bool value;
    }

    [Serializable]
    public struct Vector3DictionaryEntry
    {
        public Vector3DictionaryEntry(string key, Vector3 value)
        {
            this.key = key;
            this.value = value;
        }
        [SerializeField]
        public string key;

        [SerializeField]
        public Vector3 value;
    }
}
