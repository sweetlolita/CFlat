using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.Utility
{
    public class BiMap<TKey, TValue>
    {
        private Dictionary<TKey, TValue> dictKeyIndexed { get; set; }
        private Dictionary<TValue, TKey> dictValueIndexed { get; set; }
        private object dictLocker { get; set; }
        public BiMap()
        {
            dictKeyIndexed = new Dictionary<TKey, TValue>();
            dictValueIndexed = new Dictionary<TValue, TKey>();
            dictLocker = new object();
        }

        public void put(TKey key, TValue value)
        {
            lock (dictLocker)
            {
                if (dictKeyIndexed.ContainsKey(key))
                {
                    TValue existedValue = dictKeyIndexed[key];
                    if (dictValueIndexed.ContainsKey(existedValue))
                    {
                        dictValueIndexed.Remove(existedValue);
                    }
                    dictKeyIndexed[key] = value;
                }
                else
                {
                    dictKeyIndexed.Add(key, value);
                }

                if (dictValueIndexed.ContainsKey(value))
                {
                    TKey existedKey = dictValueIndexed[value];
                    if (dictKeyIndexed.ContainsKey(existedKey))
                    {
                        dictKeyIndexed.Remove(existedKey);
                    }
                    dictValueIndexed[value] = key;
                }
                else
                {
                    dictValueIndexed.Add(value, key);
                }
                
            }
            Logger.debug("BiMap: value {0} by key {1} has put into BiMap.",
                value, key);
        }

        public TValue searchValueByKey(TKey key)
        {
            TValue value;
            if (dictKeyIndexed.TryGetValue(key, out value) == false)
            {
                Logger.debug("BiMap: no value for key {0}.", key);
            }
            return value;
        }

        public TKey searchKeyByValue(TValue value)
        {
            TKey key;
            if (dictValueIndexed.TryGetValue(value, out key) == false)
            {
                Logger.debug("BiMap: no key for value {0}.", value);
            }
            return key;
        }

        public void removeByKey(TKey key)
        {
            try
            {
                lock (dictLocker)
                {
                    TValue value = dictKeyIndexed[key];
                    dictKeyIndexed.Remove(key);
                    dictValueIndexed.Remove(value);
                    Logger.debug("BiMap: key {0} with value {1} has removed from BiMap.",
                        key, value);
                }
            }
            catch (System.Exception ex)
            {
                Logger.debug("BiMap: key {0} removed with error {1}.",
                    key, ex.Message);
            }
        }

        public void removeByValue(TValue value)
        {
            try
            {
                lock (dictLocker)
                {
                    TKey key = dictValueIndexed[value];
                    dictValueIndexed.Remove(value);
                    dictKeyIndexed.Remove(key);
                    Logger.debug("BiMap: value {0} with key {1} has removed from BiMap.",
                        value, key);
                }
            }
            catch (System.Exception ex)
            {
                Logger.debug("BiMap: value {0} removed with error {1}.",
                    value, ex.Message);
            }
        }
    }
}
