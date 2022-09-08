using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnCommon
{
    /// <summary>
    /// <br>名前の構造体</br>
    /// <br>文字列にIDを持たせることで高速に比較ができる</br>
    /// <br>新しい文字列が使われたらリストに登録しているので、比較する際はなるべく変数として定義しておくこと</br>
    /// </summary>
    [Serializable]
    public struct Name : ISerializationCallbackReceiver
    {
        /// <summary>
        /// 文字列
        /// </summary>
        [SerializeField]
        private string name;

        /// <summary>
        /// 識別番号
        /// </summary>
        private int id;

        /// <summary>
        /// None
        /// </summary>
        public readonly static Name None = "None";

        /// <summary>
        /// コンストラクタ
        /// </summary>≥
        /// <param name="name"></param>
        public Name(string name)
        {
            if (name.IsNullOrEmpty())
            {
                this.name = "None";
            }
            else
            {
                this.name = name;
            }
            id = NameUtility.GetID(name);
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            id = NameUtility.GetID(name);
        }

        public static implicit operator string(Name name)
        {
            return name.name;
        }

        public static implicit operator Name(string name)
        {
            return new Name(name);
        }

        public static bool operator ==(Name a, Name b)
        {
            return a.id == b.id;
        }

        public static bool operator !=(Name a, Name b)
        {
            return !(a == b);
        }

        public static bool IsNone(Name name)
        {
            return name == None;
        }

        public bool IsNone()
        {
            return name == None;
        }

        public override bool Equals(object obj)
        {
            Name name = (Name)obj;
            return id == name.id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return name;
        }
    }

    /// <summary>
    /// Name構造体のユーティリティ
    /// </summary>
    public static class NameUtility
    {
        /// <summary>
        /// 登録されている文字列とIDの辞書配列
        /// </summary>
        private static Dictionary<string, int> nameToID;

        /// <summary>
        /// <br>文字列のIDを取得する</br>
        /// <br>登録済みの文字列の場合、との文字列に紐づいているIDを返す</br>
        /// <br>未登録の場合、リストの登録し、新しいIDを返す</br>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetID(string name)
        {
            // インスタンスがなかったら新しく生成
            if (nameToID == null)
            {
                nameToID = new();
            }
            if (name.IsNullOrEmpty())
            {
                name = "None";
            }
            // 要素数をIDとする
            int newValue = nameToID.Count;
            // すでに登録済みの場合、そのままValue（ID）を返す
            if (nameToID.TryGetValue(name, out int value))
            {
                //DebugLogger.Log($"Name : {name} は {value}");
                return value;
            }
            // 新しく登録し、その時の要素数をIDとして返す
            if (nameToID.TryAdd(name, newValue))
            {
                //DebugLogger.Log($"Name : {name} を {newValue} として登録");
                return newValue;
            }
            // すでに登録済みでもなく、新しく登録できない場合はエラー
            //DebugLogger.LogWarning($"Name : {name} がうまく取得できなかった");
            return -1;
        }
    }
}