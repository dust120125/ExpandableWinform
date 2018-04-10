using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu_viewer.Util
{
    public struct LRUNode<V>
    {
        public int Age;
        public V Value;

        public LRUNode(V value) : this()
        {
            Age = 0;
            Value = value;
        }
    }

    public class LRUDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _Dictionary;
        private List<TKey> KeyAgeCounter;
        private int Capacity;

        public LRUDictionary(int capacity)
        {
            _Dictionary = new Dictionary<TKey, TValue>(capacity);
            KeyAgeCounter = new List<TKey>(capacity);
            Capacity = capacity;
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue val = _Dictionary[key];
                Aging(key);
                return val;
            }
            set
            {
                if (_Dictionary.TryGetValue(key, out TValue val))
                {
                    _Dictionary[key] = value;
                    Aging(key);
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _Dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            _Dictionary.Add(key, value);
            Aging(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            bool got = _Dictionary.TryGetValue(key, out value);
            if (!got) return false;
            Aging(key);
            return true;
        }

        private void Aging(TKey key)
        {
            KeyAgeCounter.Remove(key);
            KeyAgeCounter.Add(key);
            if (KeyAgeCounter.Count <= Capacity) return;
            TKey deadKey = KeyAgeCounter[0];
            _Dictionary.Remove(deadKey);
            KeyAgeCounter.RemoveAt(0);
        }

    }
}
