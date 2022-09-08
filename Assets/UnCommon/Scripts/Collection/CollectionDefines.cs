using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UnCommon
{

    // ------------------------------------------------------------
    // intのハッシュセット
    // ------------------------------------------------------------
    [Serializable]
    public class IntHashSet : SerializableHashSet<int>
    {
        IntHashSet()
        {
            defaultValue = 0;
        }
    }

    // ------------------------------------------------------------
    // stringのハッシュセット
    // ------------------------------------------------------------
    [Serializable]
    public class StringHashSet : SerializableHashSet<string>
    {
        StringHashSet()
        {
            defaultValue = "";
        }
    }

    // ------------------------------------------------------------
    // stringのハッシュセット
    // ------------------------------------------------------------
    [Serializable]
    public class NameHashSet : SerializableHashSet<Name>
    {
        NameHashSet()
        {
            defaultValue = Name.None;
        }
    }

    // ------------------------------------------------------------
    // int to string の辞書配列
    // ------------------------------------------------------------
    [Serializable]
    public class IntToStringPair : SerializablePair<int, string> { }

    [Serializable]
    public class IntToStringDictionary : SerializableDictionay<int, string, IntToStringPair>
    {
        IntToStringDictionary()
        {
            defaultKey = 0;
        }
    }

    // ------------------------------------------------------------
    // Name to GameObject の辞書配列
    // ------------------------------------------------------------
    [Serializable]
    public class NameToGameObjectPair : SerializablePair<Name, GameObject> { }

    [Serializable]
    public class NameToGameObejctDictionary : SerializableDictionay<Name, GameObject, NameToGameObjectPair>
    {
        NameToGameObejctDictionary()
        {
            defaultKey = "None";
        }
    }

    // ------------------------------------------------------------
    // Name to AssetReference の辞書配列
    // ------------------------------------------------------------
    [Serializable]
    public class NameToAssetReferencePair : SerializablePair<Name, AssetReference> { }

    [Serializable]
    public class NameToAssetReferenceDictionary : SerializableDictionay<Name, AssetReference, NameToAssetReferencePair>
    {
        NameToAssetReferenceDictionary()
        {
            defaultKey = "None";
        }
    }

}