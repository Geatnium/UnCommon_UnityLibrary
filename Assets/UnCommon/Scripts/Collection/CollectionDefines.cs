using System;

namespace UnCommon
{
    // ------------------------------------------------------------
    // ハッシュセットのサンプル（int）
    // ------------------------------------------------------------
    [Serializable]
    public class IntHashSet : SerializableHashSet<int>
    {
        // Addするときに重複していると追加できないので、デフォルト値を決めておく
        public override void Add(int item)
        {
            if (!_collection.Contains(item))
            {
                _collection.Add(item);
            }
            else
            {
                if (!_collection.Contains(0))
                {
                    _collection.Add(0);
                }
                else
                {
                    DebugLogger.LogWarning($"要素が重複しているため追加できません。");
                }
            }
        }
    }

    // ------------------------------------------------------------
    // 辞書配列のサンプル（int to string）
    // ------------------------------------------------------------
    [Serializable]
    public class IntStringPair : SerializablePair<int, string> { }

    [Serializable]
    public class IntStringDictionary : SerializableDictionay<int, string, IntStringPair>
    {
        // Addするときに重複していると追加できないので、デフォルト値を決めておく
        public override void Add(int key, string value)
        {
            if (!_collection.ContainsKey(key))
            {
                _collection.Add(key, value);
            }
            else
            {
                if (!_collection.ContainsKey(0))
                {
                    _collection.Add(0, value);
                }
                else
                {
                    DebugLogger.LogWarning($"Keyが重複しているため追加できません。");
                }
            }
        }
    }
}