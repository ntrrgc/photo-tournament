using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoTournament
{
    class LRUCache<K, V>
    {
        public LRUCache(int maxItems)
        {
            MaxItems = maxItems;
        }

        public delegate void ElementEvictedEventHandler(K key, V value);
        public event ElementEvictedEventHandler ElementEvictedEvent;

        Dictionary<K, LinkedListNode<KeyValuePair<K, V>>> dictionary = new Dictionary<K, LinkedListNode<KeyValuePair<K, V>>>();
        LinkedList<KeyValuePair<K, V>> lruList = new LinkedList<KeyValuePair<K, V>>();

        public V TryUse(K key)
        {
            if (ContainsKey(key))
            {
                var node = dictionary[key];
                lruList.Remove(node);
                lruList.AddFirst(node);
                return node.Value.Value;
            } 
            else
                return default(V);
        }

        private void EvictLeastUsedItem()
        {
            var kv = lruList.Last.Value;
            ElementEvictedEvent?.Invoke(kv.Key, kv.Value);
            dictionary.Remove(kv.Key);
            lruList.RemoveLast();
        }

        public void Add(K key, V value)
        {
            Debug.Assert(!ContainsKey(key));
            if (dictionary.Count >= MaxItems)
                EvictLeastUsedItem();

            var node = lruList.AddFirst(new KeyValuePair<K, V>(key, value));
            dictionary[key] = node;
        }

        public bool ContainsKey(K key)
        {
            return dictionary.ContainsKey(key);
        }

        public int MaxItems { get; }
    }
}
